go get -u github.com/aws/aws-lambda-go/cmd/build-lambda-zip

$env:GOOS = "linux"
$env:CGO_ENABLED = "0"
$env:GOARCH = "amd64"
go build -o main main.go
~\Go\Bin\build-lambda-zip.exe -output function.zip main

aws lambda update-function-code --function-name dev-sm-logger-1 --zip-file fileb://function.zip

Remove-Item function.zip
Remove-Item main
