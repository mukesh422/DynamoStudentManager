using Amazon.S3;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynamoStudentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {

            private readonly IAmazonS3 _amazonS3;
            public BucketsController(IAmazonS3 amazonS3)
            {
                _amazonS3 = amazonS3;
            }

            [HttpPost]
            public async Task<IActionResult> CreateResumeBucket(string bucketName)
            {
                var bcktExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistAsync(_amazonS3, bucketName);
                if (bcktExists)
                {
                    return BadRequest($"Bucket {bucketName}already exists");
                }
                await _amazonS3.PutBucketAsync(bucketName);
                return Created("buckets", $"Bucket{bucketName}created.");


            }


            [HttpGet]

            public async Task<IActionResult> GetAllResume()
            {

                var data = await _amazonS3.ListBucketsAsync();
                var bucket = data.Buckets.Select(b => { return b.BucketName; });
                return Ok(data);

            }

            [HttpGet("{id}")]
            public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
            {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");
            await _amazonS3.DeleteObjectAsync(bucketName, key);
            return NoContent();
        }
        }
}
