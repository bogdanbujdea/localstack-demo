# LocalStack demo using .NET Core

## Setup

You have two options, either start localstack like this:

```localstack start```

or use docker-compose by running the setup.cmd for Windows or setup.sh for Linux/MacOS:

```
cd localstack
./setup.cmd
```

The setup script will add the value in SSM after starting localstack, but you can also do that using this command:

```aws --endpoint-url=http://localhost:4583 ssm put-parameter --name "/demo/settings/intervalInSeconds" --type String --value "60" --overwrite --region "us-east-1"```

To query the value, use this command:

```aws ssm --endpoint-url=http://localhost:4583 get-parameter --name "/demo/settings/intervalInSeconds"```

## Tools:

[LocalStack](https://github.com/localstack/localstack)

[Commandeer](https://getcommandeer.com/)