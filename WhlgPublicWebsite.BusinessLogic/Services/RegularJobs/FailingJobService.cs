namespace WhlgPublicWebsite.BusinessLogic.Services.RegularJobs;

public class FailingJobService
{
    public void Run()
    {
        throw new Exception("This job always fails");
    }
}