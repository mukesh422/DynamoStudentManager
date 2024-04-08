using Amazon.S3;
using Amazon.S3.Model;
using DynamoStudentManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace DynamoStudentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;

        public FilesController(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }


        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _amazonS3.PutObjectAsync(request);
            return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFiles(string bucketName, string? prefix)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists)
            {
                return NotFound($"Bucket {bucketName} does not exist.");
            }

            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };
            var result = await _amazonS3.ListObjectsV2Async(request);
            var S3ResumeObj = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                return new S3ResumeObj()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = _amazonS3.GetPreSignedURL(urlRequest),
                };
            });
            return Ok(S3ResumeObj);

        }

        [HttpGet("preview")]
        public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
        {
            try
            {
                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
                if (!bucketExists)
                    return NotFound($"Bucket {bucketName} does not exist.");

                var s3Object = await _amazonS3.GetObjectAsync(bucketName, key);
                return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error occurred: {ex.Message}");

                // Return a 500 Internal Server Error response
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFile(string bucketName, string key)
        {
            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist");
            await _amazonS3.DeleteObjectAsync(bucketName, key);
            return NoContent();
        }



    }
}
