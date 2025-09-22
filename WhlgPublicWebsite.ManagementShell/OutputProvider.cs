namespace WhlgPublicWebsite.ManagementShell;

public interface IOutputProvider
{
    public void Output(string outputString);

    public bool Confirm(string outputString);

    public string GetString(string outputString);
}

public class OutputProvider : IOutputProvider
{
    public void Output(string outputString)
    {
        Console.WriteLine(outputString);
    }

    public bool Confirm(string outputString)
    {
        return GetString(outputString) == "y";
    }

    public string GetString(string outputString)
    {
        Console.WriteLine(outputString);
        var inputString = Console.ReadLine();
        return inputString?.Trim();
    }
}