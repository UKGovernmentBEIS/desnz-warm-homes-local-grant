using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using HerPublicWebsite.BusinessLogic.Services.S3ReferralFileKeyGenerator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;

public class S3FileWriter : IS3FileWriter
{
    private readonly S3FileWriterConfiguration config;
    private readonly ILogger logger;
    private readonly S3ReferralFileKeyGenerator keyGenerator;
    private readonly IWebHostEnvironment environment;

    public S3FileWriter(
        IOptions<S3FileWriterConfiguration> options,
        ILogger<S3FileWriter> logger,
        S3ReferralFileKeyGenerator keyGenerator,
        IWebHostEnvironment environment)
    {
        this.config = options.Value;
        this.logger = logger;
        this.keyGenerator = keyGenerator;
        this.environment = environment;
    }

    // Ideally we'd stream this file to S3 as we generate it. However S3 needs to know the file size up front so we
    // can't do that. Given the file sizes we'll be dealing with that shouldn't be a problem though.
    // We'll still use a stream here so that if the AWS SDK ever starts supporting streams that don't know their length
    // it will be easier to take advantage.
    // https://github.com/aws/aws-sdk-net/issues/675
    public async Task WriteFileAsync(string custodianCode, int month, int year, Stream fileContent)
    {
        var keyName = keyGenerator.GetS3Key(custodianCode, month, year);
        
        try
        {
            AmazonS3Client s3Client;
            if (environment.IsEnvironment("Development"))
            {
                // For local development connect to a local instance of Minio
                var clientConfig = new AmazonS3Config
                {
                    AuthenticationRegion = config.Region,
                    ServiceURL = config.LocalDevOnly_ServiceUrl,
                    ForcePathStyle = true,
                };
                s3Client = new AmazonS3Client(config.LocalDevOnly_AccessKey, config.LocalDevOnly_SecretKey, clientConfig);
            }
            else
            {
                s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(config.Region));
            }

            var fileTransferUtility = new TransferUtility(s3Client);

            await fileTransferUtility.UploadAsync(fileContent, config.BucketName, keyName);
        }
        catch (AmazonS3Exception e)
        { 
            logger.LogError("AWS S3 error when writing CSV file to bucket: '{0}', key: '{1}'. Message:'{2}'", config.BucketName, keyName, e.Message);
            throw;
        }
        catch (Exception e)
        {
            logger.LogError("Error when writing CSV file to bucket: '{0}', key: '{1}'. Message:'{2}'", config.BucketName, keyName, e.Message);
            throw;
        }
    }
}
