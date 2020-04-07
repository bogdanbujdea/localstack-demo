using Amazon.Runtime;
using Amazon.S3;
using localstack_demo.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace localstack_demo
{
    public class Startup
    {
        private readonly IHostEnvironment _hostEnvironment;

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<IBucketRepository, BucketRepository>();

            if (_hostEnvironment.IsDevelopment())
            {
                var amazonS3 = new AmazonS3Client(new BasicAWSCredentials("abc", "def"), new AmazonS3Config
                {
                    ServiceURL = Consts.S3ServiceUrl,
                    ForcePathStyle = true,
                    UseHttp = true
                });

                services.AddTransient(typeof(IAmazonS3), provider => amazonS3);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
