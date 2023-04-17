namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public interface IEpcApi
    {
        public Task<EpcAssessment> EpcFromUprn(string uprn); // TODO: Create real EPC methods
    }
}
