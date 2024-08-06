using HerPublicWebsite.BusinessLogic;
using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.ManagementShell;

public class CommandHandler
{
    private IDatabaseOperation databaseOperation;
    private IFakeReferralGenerator fakeReferralGenerator;
    private IOutputProvider outputProvider;
    
    public CommandHandler(IDatabaseOperation databaseOperation, IFakeReferralGenerator fakeReferralGenerator, IOutputProvider outputProvider)
    {
        this.databaseOperation = databaseOperation;
        this.fakeReferralGenerator = fakeReferralGenerator;
        this.outputProvider = outputProvider;
    }

    public void GenerateReferrals(string[] args)
    {
        if (args.Length == 0)
        {
            outputProvider.Output("need a number please");
            return;
        }

        if (!int.TryParse(args[0], out var referralCount))
        {
            outputProvider.Output("thats a string");
        }

        var referralsToAdd = fakeReferralGenerator.GenerateFakeReferralRequests(referralCount);

        databaseOperation.AddReferralRequests(referralsToAdd);
    }
}