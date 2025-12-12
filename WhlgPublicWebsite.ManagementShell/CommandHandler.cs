using WhlgPublicWebsite.BusinessLogic.Models;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.Data;

namespace WhlgPublicWebsite.ManagementShell;

public class CommandHandler(
    IDatabaseOperation databaseOperation,
    IFakeReferralGenerator fakeReferralGenerator,
    IOutputProvider outputProvider,
    ICsvFileCreator csvFileCreator)
{
    private const string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";

    public void GenerateReferrals(string[] args)
    {
        if (args.Length == 0)
        {
            outputProvider.Output("Missing number of referrals. Usage: 'GenerateReferrals <count>'.");
            return;
        }

        if (!int.TryParse(args[0], out var referralCount))
            outputProvider.Output("Invalid number of referrals. Usage: 'GenerateReferrals <count>'.");

        if (FlagAndReturnIfRunningOnProd()) return;

        outputProvider.Output("!!!!!!!!!!!!!!!!!!!!!!");
        outputProvider.Output(
            $"You are about to generate {referralCount} fake referral requests and add them to the database.");

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

    public void GeneratePerMonthStatistics(string[] args)
    {
        outputProvider.Output(
            "This function will output a CSV file to the terminal for you to copy into a local file.");

        AuthorityTypeSubcommand statisticsTypeSubcommand;

        try
        {
            statisticsTypeSubcommand = Enum.Parse<AuthorityTypeSubcommand>(args[0].Trim(), true);
        }
        catch (Exception e) when (e is ArgumentException or IndexOutOfRangeException)
        {
            var allSubcommands = string.Join("/", Enum.GetValues<AuthorityTypeSubcommand>());
            outputProvider.Output(
                $"Please specify a valid statistics type - Usage: GeneratePerMonthStatistics <{allSubcommands}>");
            return;
        }

        outputProvider.Output("Retrieving all WH:LG referrals submitted after HUG2 Shutdown.");
        var referralRequests = databaseOperation.GetAllWhlgReferralRequestsSubmittedAfterHug2Shutdown();
        outputProvider.Output("WH:LG Referrals retrieved successfully");

        MemoryStream referralStatistics = null;
        switch (statisticsTypeSubcommand)
        {
            case AuthorityTypeSubcommand.LocalAuthority:
                outputProvider.Output("Generating referrals per Local Authority per month CSV.");
                referralStatistics =
                    csvFileCreator.CreatePerMonthLocalAuthorityReferralStatisticsForConsole(referralRequests);
                break;
            case AuthorityTypeSubcommand.Consortium:
                outputProvider.Output("Generating referrals per Consortium per month CSV.");
                referralStatistics =
                    csvFileCreator.CreatePerMonthConsortiumReferralStatisticsForConsole(referralRequests);
                break;
        }

        outputProvider.Output("\n" + MemoryStreamHelper.MemoryStreamToString(referralStatistics));
        outputProvider.Output("Output Complete.");
    }

    public async Task ExportNewReferralRequestsToPortal(WhlgDbContext context)
    {
        if (FlagAndReturnIfRunningOnProd()) return;

        // "DEV" is used for deployed development environments
        // "Development" is used for local development environments
        // No connection to AWS is possible in local dev so prevent running this command.
        if (!(GetEnvironment() == "DEV" || GetEnvironment() == "Staging"))
        {
            outputProvider.Output(
                $"This command can only run with \"{EnvironmentKey}\" set to \"DEV\" or \"Staging\".");
            outputProvider.Output(
                "We expect to find this configuration on Development & Staging deployed environments respectively.");
            outputProvider.Output("This command cannot be run locally.");
            return;
        }

        if (Environment.GetEnvironmentVariable("S3__BucketName") == null ||
            Environment.GetEnvironmentVariable("S3__Region") == null)
        {
            outputProvider.Output(
                "This command requires the S3__BucketName and S3__Region environment variables to be set.");
            outputProvider.Output(
                "We expect these to be the variables used by the program in S3Configuration to connect to the S3 bucket.");
            return;
        }

        outputProvider.Output(
            "This command will export all referral requests received since the last export to the referrals S3 bucket.");
        outputProvider.Output(
            "This command should only be run if a new referral request has been submitted and needs to be available in the Portal immediately.");
        outputProvider.Output("Please refer to the documentation if unsure.");
        var confirmation = outputProvider.Confirm("Would you like to continue? (Y/N)");

        if (!confirmation)
        {
            outputProvider.Output("No referrals exported");
            return;
        }

        // construct an instance and draw config from environment variables
        var unsubmittedReferralRequestsService = new CommandLineUnsubmittedReferralRequestsService(context);

        outputProvider.Output("Writing new referral requests to AWS bucket.");
        await unsubmittedReferralRequestsService.WriteUnsubmittedReferralRequestsToCsv();
        outputProvider.Output("Unsubmitted referral requests written to AWS bucket successfully.");
        outputProvider.Output("They should now be available to view in the Portal.");
    }

    public async Task SetEmergencyMaintenanceState(string[] args, WhlgDbContext context)
    {
        DisplayMaintenanceStateWarnings();

        var commandLineEmergencyMaintenanceService = new CommandLineEmergencyMaintenanceService(context);
        EmergencyMaintenanceState? argEmergencyMaintenanceState = ParseEmergencyMaintenanceStateInput(args);
        if (argEmergencyMaintenanceState is null) return;

        var emergencyMaintenanceVerb =
            argEmergencyMaintenanceState == EmergencyMaintenanceState.Enabled ? "ENABLE" : "DISABLE";
        var liveEmergencyMaintenanceState = await commandLineEmergencyMaintenanceService.GetEmergencyMaintenanceState();

        DisplayMaintenanceStateDetails(emergencyMaintenanceVerb, liveEmergencyMaintenanceState);

        if (argEmergencyMaintenanceState == liveEmergencyMaintenanceState)
        {
            outputProvider.Output("The requested state is the same as the current state.");
            outputProvider.Output("Exiting without changes...");
            return;
        }

        var confirmation = GetUserConfirmationForSettingMaintenanceState(emergencyMaintenanceVerb);
        if (!confirmation) return;

        var authorEmail = GetUserEmailForAudit();
        if (authorEmail is null) return;

        await commandLineEmergencyMaintenanceService.SetEmergencyMaintenanceState((EmergencyMaintenanceState)argEmergencyMaintenanceState, authorEmail);

        outputProvider.Output("Output Complete.");
    }

    private string? GetUserEmailForAudit()
    {
        var authorEmail = outputProvider.GetString("Please enter your email address for the audit record:");

        if (string.IsNullOrWhiteSpace(authorEmail))
        {
            outputProvider.Output("No email address entered.");
            outputProvider.Output("Exiting without changes...");
            return null;
        }
        
        return authorEmail;
    }
    
    private bool GetUserConfirmationForSettingMaintenanceState(string emergencyMaintenanceVerb)
    {
        outputProvider.Output("!!!!!!!!!!!!!!!!!!!!!!");
        outputProvider.Output(
            $"Please confirm you would like to {emergencyMaintenanceVerb} emergency maintenance mode.");
        var confirmation = outputProvider.Confirm("Would you like to continue? (Y/N)");

        if (!confirmation)
        {
            outputProvider.Output("Exiting without changes...");
        }

        return confirmation;
    }

    private void DisplayMaintenanceStateDetails(string emergencyMaintenanceVerb, EmergencyMaintenanceState liveEmergencyMaintenanceState)
    {
        var isMaintenanceStateEnabled = liveEmergencyMaintenanceState == EmergencyMaintenanceState.Enabled;

        outputProvider.Output("Details:");
        outputProvider.Output($"Request is to {emergencyMaintenanceVerb} emergency maintenance mode.");
        outputProvider.Output($"Portal is currently {(isMaintenanceStateEnabled ? "IN" : "NOT IN")} emergency maintenance mode. Referrals cannot be submitted.");
    }

    private void DisplayMaintenanceStateWarnings()
    {
        outputProvider.Output("!!!!!!!!!!!!!!!!!!!!!!");
        outputProvider.Output("This is a dangerous operation! Read the following before proceeding:");
        outputProvider.Output("This function will enable or disable emergency maintenance mode.");
        outputProvider.Output(
            "In emergency maintenance mode, all requests to this portal will be blocked with a 503 response.");
        outputProvider.Output(
            "This should only be used in accordance with our disaster response plan when usage of the site by members of the public needs to be blocked.");
        outputProvider.Output("If unsure, quit and consult the documentation.");
        outputProvider.Output("!!!!!!!!!!!!!!!!!!!!!!");
    }

    private EmergencyMaintenanceState? ParseEmergencyMaintenanceStateInput(string[] args)
    {
        try
        {
            return Enum.Parse<EmergencyMaintenanceState>(args[0].Trim(), ignoreCase: true);
        }
        catch (Exception e) when (e is ArgumentException or IndexOutOfRangeException)
        {
            var allSubcommands = string.Join("/", Enum.GetValues<EmergencyMaintenanceState>());
            outputProvider.Output(
                $"Please specify whether to enable or disable emergency mode - Usage: SetEmergencyMaintenanceState <{allSubcommands}>");
            outputProvider.Output("Exiting without changes...");
            return null;
        }
    }

    private string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable(EnvironmentKey);
    }

    private bool FlagAndReturnIfRunningOnProd()
    {
        var environment = GetEnvironment();

        if (environment == "Production")
        {
            outputProvider.Output("Detected that the current environment is Production. Terminating.");
            return true;
        }

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
            outputProvider.Output(
                $"Expecting to find this information in \"{EnvironmentKey}\" environment variable.");
            outputProvider.Output("If this is no longer up to date please raise a ticket to fix this.");
            outputProvider.Output("");
        }

        return false;
    }

    private enum AuthorityTypeSubcommand
    {
        LocalAuthority,
        Consortium
    }
}