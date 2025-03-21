using System.Text;

namespace WhlgPublicWebsite.ManagementShell;

public static class MemoryStreamHelper
{
    public static string MemoryStreamToString(MemoryStream memoryStream)
    {
        memoryStream.Seek(0, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}