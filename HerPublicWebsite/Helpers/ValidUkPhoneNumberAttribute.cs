using System;
using System.ComponentModel.DataAnnotations;
using PhoneNumbers;

namespace HerPublicWebsite.Helpers;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ValidUkPhoneNumberAttribute : DataTypeAttribute
{
    public string ValidateIf;
    
    public ValidUkPhoneNumberAttribute() : base(DataType.PhoneNumber)
    {
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var validateIfPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(ValidateIf);
            
        if (validateIfPropertyInfo is null)
        {
            throw new ArgumentException(
                $"'{ValidateIf}' must be a boolean property in the model the attribute is included in");
        }
            
        var doValidation = (bool)validateIfPropertyInfo.GetValue(validationContext.ObjectInstance, null)!;
            
        if (!doValidation)
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
