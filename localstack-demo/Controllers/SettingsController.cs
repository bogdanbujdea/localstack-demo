using System;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace localstack_demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingsController : ControllerBase
    {
        private AmazonSimpleSystemsManagementClient _amazonSettingsClient;


        public SettingsController(IHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                var systemsManagementConfig = new AmazonSimpleSystemsManagementConfig
                {
                    ServiceURL = Consts.SSMServiceUrl,
                    UseHttp = true
                };
                _amazonSettingsClient = new AmazonSimpleSystemsManagementClient(new BasicAWSCredentials("abc", "def"), systemsManagementConfig);
            }
        }

        [HttpGet("interval")]
        public async Task<ActionResult> GetIntervalInSeconds()
        {
            try
            {
                var getParameterRequest = new GetParameterRequest
                {
                    Name = $"/{Consts.ParameterStoreName}/settings/intervalInSeconds",
                };
                var response = await _amazonSettingsClient.GetParameterAsync(getParameterRequest);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                    return Ok(response.Parameter);
                return BadRequest("Couldn't retrieve the value");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpGet("setinterval")]
        public async Task<ActionResult> SetIntervalInSeconds([FromQuery]int seconds)
        {
            try
            {
                var putParameterRequest = new PutParameterRequest
                {
                    Name = $"/{Consts.ParameterStoreName}/settings/intervalInSeconds",
                    Value = seconds.ToString(),
                    Type = ParameterType.String,
                    Overwrite = true
                };
                var response = await _amazonSettingsClient.PutParameterAsync(putParameterRequest);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                    return Ok(response);
                return BadRequest("Couldn't update the value");
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
