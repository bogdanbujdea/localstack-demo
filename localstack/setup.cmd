REM Keep this at the end of each AWS call: "& ^"
REM More details here: https://stackoverflow.com/questions/4036754/why-does-only-the-first-line-of-this-windows-batch-file-execute-but-all-three-li
docker-compose up -d
aws --endpoint-url=http://localhost:4583 ssm put-parameter --name "/demo/settings/intervalInSeconds" --type String --value "60" --overwrite --region "us-east-1" & ^
pause