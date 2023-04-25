namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public interface IEpcApi
    {
        public Task<EpcAssessmentDto> EpcFromUprn(string uprn);
    }
}
