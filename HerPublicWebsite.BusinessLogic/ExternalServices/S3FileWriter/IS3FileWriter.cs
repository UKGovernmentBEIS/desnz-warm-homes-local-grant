namespace HerPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;

public interface IS3FileWriter
{
    public Task WriteFileAsync(int custodianCode, int month, int year, Stream fileContent);
}
