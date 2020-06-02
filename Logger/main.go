package main

import (
	"bytes"
	"context"
	"fmt"
	"io"
	"os"
	"strings"
	"time"

	"github.com/aws/aws-lambda-go/events"
	"github.com/aws/aws-lambda-go/lambda"
	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/session"
	"github.com/aws/aws-sdk-go/service/s3"
)

var (
	bucketName = os.Getenv("LOG_BUCKET_NAME")
)

func main() {
	lambda.Start(handler)
}

func handler(ctx context.Context, evt events.SQSEvent) error {
	sess, err := session.NewSession()
	if err != nil {
		return fmt.Errorf("failed to instantiate a new session: %v", err)
	}

	return log(sess, evt.Records)
}

// log a collection of SQSMessages.
func log(sess *session.Session, messages []events.SQSMessage) error {
	now := time.Now().UTC()
	key := fmt.Sprintf("%d-%d-%d", now.Year(), now.Month(), now.Day())

	api := s3.New(sess)

	// Download file.
	file, err := downloadFile(api, key)
	if err != nil {
		return err
	}

	// Write message to file.
	w := bytes.NewBuffer(file)
	for _, m := range messages {
		_, err = w.WriteString(m.Body + "\n")
		if err != nil {
			return err
		}
	}

	// Upload file.
	err = uploadFile(api, key, w.Bytes())
	if err != nil {
		return err
	}

	return nil
}

// downloadFile attempts to download an S3 object. If an object with
// the given key doesn't exist, an empty []byte is returned.
func downloadFile(api *s3.S3, key string) ([]byte, error) {
	resp, err := api.GetObject(&s3.GetObjectInput{
		Bucket: aws.String(bucketName),
		Key:    aws.String(key),
	})
	if err != nil {
		if strings.Contains(err.Error(), s3.ErrCodeNoSuchKey) {
			return []byte{}, nil
		}

		return nil, fmt.Errorf("failed to download file: %v", err)
	}

	buf := make([]byte, *resp.ContentLength)
	_, err = resp.Body.Read(buf)
	if err != nil {
		if err == io.EOF {
			return []byte{}, nil
		}

		return nil, fmt.Errorf("failed to read file: %v", err)
	}
	defer resp.Body.Close()

	return buf, nil
}

// uploadFile attempts to PUT an object in S3. If an object doesn't exist
// with the given key, the data is uploaded as a new part.
func uploadFile(api *s3.S3, key string, data []byte) error {
	_, err := api.PutObject(&s3.PutObjectInput{
		Bucket: aws.String(bucketName),
		Key:    aws.String(key),
		Body:   bytes.NewReader(data),
	})
	if err != nil {
		if !strings.Contains(err.Error(), s3.ErrCodeNoSuchKey) {
			return fmt.Errorf("failed to put object: %v", err)
		}

		_, err := api.UploadPart(&s3.UploadPartInput{
			Bucket: aws.String(bucketName),
			Key:    aws.String(key),
			Body:   bytes.NewReader(data),
		})
		if err != nil {
			return fmt.Errorf("failed to upload part: %v", err)
		}
	}

	return nil
}
