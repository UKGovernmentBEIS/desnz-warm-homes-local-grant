using System.Text;

namespace WhlgPublicWebsite.ManagementShell;

public interface IMemoryStreamHelper
{
    public string MemoryStreamToString(MemoryStream memoryStream);
}

public class MemoryStreamHelper : IMemoryStreamHelper
{
    public string MemoryStreamToString(MemoryStream memoryStream)
    {
        memoryStream.Seek(0, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}