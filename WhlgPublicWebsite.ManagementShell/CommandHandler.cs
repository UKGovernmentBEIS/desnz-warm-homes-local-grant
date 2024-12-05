namespace WhlgPublicWebsite.ManagementShell;

public class CommandHandler
{
    private readonly IDatabaseOperation databaseOperation;
    private readonly IFakeReferralGenerator fakeReferralGenerator;
    private readonly IOutputProvider outputProvider;

    public CommandHandler(IDatabaseOperation databaseOperation, IFakeReferralGenerator fakeReferralGenerator,
        IOutputProvider outputProvider)
    {
        this.databaseOperation = databaseOperation;
        this.fakeReferralGenerator = fakeReferralGenerator;
        this.outputProvider = outputProvider;
    }

    public void GenerateReferrals(string[] args)
    {
        if (args.Length == 0)
        {
            outputProvider.Output("Missing number of referrals. Usage: 'GenerateReferrals <count>'.");
            return;
        }

        if (!int.TryParse(args[0], out var referralCount))
            outputProvider.Output("Invalid number of referrals. Usage: 'GenerateReferrals <count>'.");

        const string environmentKey = "ASPNETCORE_ENVIRONMENT";
        var environment = Environment.GetEnvironmentVariable(environmentKey);

        if (environment == "Production")
        {
            outputProvider.Output("Detected that the current environment is Production. Terminating.");
        }

        outputProvider.Output("!!!!!!!!!!!!!!!!!!!!!!");
        outputProvider.Output(
            $"You are about to generate {referralCount} fake referral requests and add them to the database.");
        outputProvider.Output("This command should only be run on development and staging environments.");
        outputProvider.Output("Double check that you are NOT running this command on a production environment.");
        outputProvider.Output("");
        if (environment != null)
        {
            outputProvider.Output($"Current detected environment: {environment}");
        }
        else
        {
            outputProvider.Output("Could not determine current environment.");
            outputProvider.Output($"Expecting to find this information in \"{environmentKey}\" environment variable.");
            outputProvider.Output("If this is no longer up to date please raise a ticket to fix this.");
        }
        outputProvider.Output("");
        outputProvider.Output("All users will have a FullName that begins \"FAKE USER\".");
        outputProvider.Output("To revert, connect to the database and run the following:");
        outputProvider.Output("DELETE FROM \"ReferralRequests\" WHERE \"FullName\" LIKE 'FAKE USER %';");
        outputProvider.Output("!!!!!!!!!!!!!!!!!!!!!!");
        var confirmation = outputProvider.Confirm("Would you like to continue? (Y/N)");

        if (!confirmation)
        {
            outputProvider.Output("No referrals generated");
            return;
        }

        var referralsToAdd = fakeReferralGenerator.GenerateFakeReferralRequests(referralCount);

        databaseOperation.AddReferralRequests(referralsToAdd);
    }
}