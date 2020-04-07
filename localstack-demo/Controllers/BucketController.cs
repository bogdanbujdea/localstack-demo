using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace localstack_demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BucketController: ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateBucket([FromBody] BucketInfo bucketInfo)
        {
            var createResponse = await _bucketRepository.CreateBucket(bucketInfo.Name);
            if (createResponse.IsSuccess)
            {
                return Ok(createResponse.Value);
            }
            else
            {
                return BadRequest(createResponse.Error);
            }
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadFile([FromBody] FileInfo fileInfo)
        {
            var result = await _bucketRepository.UploadFiles(fileInfo);
            if (result.IsSuccess)
            {
                return Ok("Success");
            }
            else
            {
                return BadRequest("Error: " + result.Error);
            }
        }
    }

    public class BucketInfo
    {
        public string Name { get; set; }
    }

    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _amazonS3;

        public BucketRepository(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        public async Task<Result<CreateBucketResponse>> CreateBucket(string bucketName)
        {
            try
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                    CannedACL = S3CannedACL.NoACL
                };
                var response = await _amazonS3.PutBucketAsync(putBucketRequest);

                return Result.Ok(new CreateBucketResponse
                {
                    BucketName = bucketName,
                    RequestId = response.ResponseMetadata.RequestId
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Result> UploadFiles(FileInfo fileInfo)
        {
            var bytes = File.ReadAllBytes(fileInfo.FilePath);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = new MemoryStream(bytes),
                Key = fileInfo.FileName,
                BucketName = fileInfo.BucketName,
                CannedACL = S3CannedACL.NoACL
            };

            using (var fileTransferUtility = new TransferUtility(_amazonS3))
            {
                await fileTransferUtility.UploadAsync(uploadRequest);
            }

            return Result.Ok();
        }
    }

    public interface IBucketRepository
    {
        Task<Result<CreateBucketResponse>> CreateBucket(string bucketName);

        Task<Result> UploadFiles(FileInfo fileInfo);
    }

    public class CreateBucketResponse
    {
        public string RequestId { get; set; }

        public string BucketName { get; set; }
    }

    public class FileInfo
    {
        public string BucketName { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
