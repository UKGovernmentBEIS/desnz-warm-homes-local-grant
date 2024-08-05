using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.ManagementShell;

public class CommandHandler
{
    private IOutputProvider outputProvider;
    private IDatabaseOperation databaseOperation;
    
    public CommandHandler(IDatabaseOperation databaseOperation, IOutputProvider outputProvider)
    {
        this.databaseOperation = databaseOperation;
        this.outputProvider = outputProvider;
    }

    public void GenerateReferrals(string[] args)
    {
        if (args.Length == 0)
        {
            outputProvider.Output("need a number please");
            return;
        }

        int referralCount;
        if (!int.TryParse(args[0], out referralCount))
        {
            outputProvider.Output("thats a string");
        }

        // TODO: generate them
        var referralsToAdd = new List<ReferralRequest>();

        databaseOperation.AddReferralRequests(referralsToAdd);
    }
}