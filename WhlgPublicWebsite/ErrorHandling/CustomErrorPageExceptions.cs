using System;

namespace HerPublicWebsite.ErrorHandling
{

    public abstract class CustomErrorPageException : Exception
    {
        public abstract string ViewName { get; }
        public abstract int StatusCode { get; }
    }


    public class PropertyReferenceNotFoundException : CustomErrorPageException
    {
        public override string ViewName => "../Error/PropertyReferenceNotFound";
        public override int StatusCode => 404;
        public string Reference { get; set; }
    }

}
