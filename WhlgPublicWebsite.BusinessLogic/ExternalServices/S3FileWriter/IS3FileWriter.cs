namespace WhlgPublicWebsite.BusinessLogic.ExternalServices.S3FileWriter;

public interface IS3FileWriter
{
    public Task WriteFileAsync(string custodianCode, int month, int year, Stream fileContent);
}
