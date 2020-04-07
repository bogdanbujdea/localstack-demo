using System;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace localstack_demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    if (env.EnvironmentName == "Development")
                    {
                        builder.AddSystemsManager($"/{Consts.ParameterStoreName}", new AWSOptions
                        {
                            DefaultClientConfig =
                            {
                                ServiceURL = Consts.SSMServiceUrl,
                                UseHttp = true
                            },
                            Credentials = new BasicAWSCredentials("abc", "def")
                        }, TimeSpan.FromSeconds(30));
                    }
                    else
                    {
                        builder.AddSystemsManager($"/{Consts.ParameterStoreName}", new AWSOptions(),
                            TimeSpan.FromSeconds(30));
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
