using Microsoft.EntityFrameworkCore;
using WhlgPublicWebsite.BusinessLogic.Services.CsvFileCreator;
using WhlgPublicWebsite.Data;

namespace WhlgPublicWebsite.ManagementShell;

public static class Program
{
    public static void Main(string[] args)
    {
        var outputProvider = new OutputProvider();
        var contextOptions = new DbContextOptionsBuilder<WhlgDbContext>()
            .UseNpgsql(
                Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSQLConnection") ??
                @"UserId=postgres;Password=postgres;Server=localhost;Port=5432;Database=whlgdev;Include Error Detail=true;Pooling=true")
            .Options;
        using var context = new WhlgDbContext(contextOptions);
        var csvFileCreator = new CsvFileCreator();
        var databaseOperation = new DatabaseOperation(context, outputProvider);
        var fakeReferralGenerator = new FakeReferralGenerator();
        var commandHandler =
            new CommandHandler(databaseOperation, fakeReferralGenerator, outputProvider, csvFileCreator);

        Subcommand command;
        string[] subcommandArgs;

        try
        {
            command = Enum.Parse<Subcommand>(args[0], true);
            subcommandArgs = args.Skip(1).ToArray();
        }
        catch (Exception)
        {
            var allSubcommands = string.Join(", ", Enum.GetValues<Subcommand>());
            outputProvider.Output(
                $"Please specify a valid subcommand - available options are: {allSubcommands}");
            return;
        }

        switch (command)
        {
            case Subcommand.GenerateReferrals:
                commandHandler.GenerateReferrals(subcommandArgs);
                return;
            case Subcommand.GeneratePerMonthStatistics:
                commandHandler.GeneratePerMonthStatistics(subcommandArgs);
                return;
            default:
                outputProvider.Output("Invalid terminal command entered. Please refer to the documentation");
                return;
        }
    }

    private enum Subcommand
    {
        GenerateReferrals,
        GeneratePerMonthStatistics
    }
}