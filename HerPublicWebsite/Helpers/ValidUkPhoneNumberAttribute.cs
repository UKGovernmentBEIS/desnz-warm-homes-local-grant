using System;
using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace HerPublicWebsite.Helpers;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ValidUkPhoneNumberAttribute : DataTypeAttribute
{
    public string DoNotValidateIf;
    
    public ValidUkPhoneNumberAttribute() : base(DataType.PhoneNumber)
    {
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var doNotValidatePropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(DoNotValidateIf);
            
        if (doNotValidatePropertyInfo is null)
        {
            throw new ArgumentException(
                $"'{DoNotValidateIf}' must be a boolean property in the model the attribute is included in");
        }
            
        var doNotValidate = (bool)doNotValidatePropertyInfo.GetValue(validationContext.ObjectInstance, null)!;
            
        if (doNotValidate)
        {
            return ValidationResult.Success;
        }

        if (value is not string valueAsString)
        {
            return new ValidationResult(ErrorMessage);
        }

        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        try
        {
            var parsedPhoneNumber = phoneNumberUtil.Parse(valueAsString, "GB");
            return phoneNumberUtil.IsValidNumber(parsedPhoneNumber) ?
                    ValidationResult.Success :
                    new ValidationResult(ErrorMessage);
        }
        catch (NumberParseException)
        {
            return new ValidationResult(ErrorMessage);
        }
    }
}
