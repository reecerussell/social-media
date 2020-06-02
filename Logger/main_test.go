package main

import (
	"testing"

	"github.com/aws/aws-sdk-go/aws"
	"github.com/aws/aws-sdk-go/aws/credentials"
	"github.com/aws/aws-sdk-go/aws/session"
)

func TestLog(t *testing.T) {
	bucketName = "dev-sm-logs-1"
	sess, err := session.NewSession(&aws.Config{
		Region:      aws.String("eu-west-2"),
		Credentials: credentials.NewSharedCredentials("C:\\Users\\me\\.aws\\credentials", "Reece Russell"),
	})
	if err != nil {
		t.Fatal(err)
		return
	}

	err = log(sess, "test message")
	if err != nil {
		t.Errorf("expected nil, but got: %v", err)
	}
}
