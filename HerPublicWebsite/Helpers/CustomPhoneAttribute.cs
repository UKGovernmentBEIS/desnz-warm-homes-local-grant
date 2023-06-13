using System;
using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace HerPublicWebsite.Helpers;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class CustomPhoneNumberAttribute : DataTypeAttribute
{
    public CustomPhoneNumberAttribute() : base(DataType.PhoneNumber)
    {
    }

    public override bool IsValid(object value)
    {
        if (value is not string valueAsString)
        {
            return false;
        }

        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        try
        {
            var parsedPhoneNumber = phoneNumberUtil.Parse(valueAsString, "GB");
            return phoneNumberUtil.IsValidNumber(parsedPhoneNumber);
        }
        catch (NumberParseException)
        {
            return false;
        }
    }
}
