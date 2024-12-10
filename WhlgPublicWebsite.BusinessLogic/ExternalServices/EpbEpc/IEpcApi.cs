﻿using HerPublicWebsite.BusinessLogic.Models;

namespace HerPublicWebsite.BusinessLogic.ExternalServices.EpbEpc
{
    public interface IEpcApi
    {
        public Task<EpcDetails> EpcFromUprnAsync(string uprn);
    }
}
