using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhlgPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;
using WhlgPublicWebsite.BusinessLogic.Services.S3ReferralFileKeyGenerator;
using WhlgPublicWebsite.Data;

namespace WhlgPublicWebsite.ManagementShell;

/**
 * Wraps the mechanism required to create an instance of the UnsubmittedReferralRequestsService.
 * For use only with the CLI code, otherwise use DI to get an instance of IUnsubmittedReferralRequestsService.
 */
public class CommandLineUnsubmittedReferralRequestsService
{
    private UnsubmittedReferralRequestsService UnsubmittedReferralRequestsService { get; }

    public CommandLineUnsubmittedReferralRequestsService(WhlgDbContext context)
    {
        var dataAccessProvider = new DataAccessProvider(context);
        var s3Config = new S3Configuration
        {
            BucketName = Environment.GetEnvironmentVariable("S3__BucketName"),
            Region = Environment.GetEnvironmentVariable("S3__Region")
        };
        var s3FileWriterLogger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<S3FileWriter>();
        var s3ReferralFileKeyGenerator = new S3ReferralFileKeyGenerator();
        var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(s3Config.Region));
        var s3FileWriter = new S3FileWriter(Options.Create(s3Config), s3FileWriterLogger, s3ReferralFileKeyGenerator,
            s3Client);
        var csvFileCreator = new CsvFileCreator();
        UnsubmittedReferralRequestsService =
            new UnsubmittedReferralRequestsService(dataAccessProvider, s3FileWriter, csvFileCreator);
    }

    public Task WriteUnsubmittedReferralRequestsToCsv()
    {
        return UnsubmittedReferralRequestsService.WriteUnsubmittedReferralRequestsToCsv();
    }
}