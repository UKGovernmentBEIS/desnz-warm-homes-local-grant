using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WhlgPublicWebsite.Helpers;

/// <summary>
/// Stricter than <see cref="EmailAddressAttribute"/>: rejects addresses with whitespace or commas
/// (common typos seen in WH:LG data, e.g. comma instead of a dot in the domain).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public partial class ValidEmailAddressAttribute : DataTypeAttribute
{
    [GeneratedRegex(@"^[^\s@]+@(?:[A-Za-z0-9-]+\.)+[A-Za-z]{2,}$")]
    private static partial Regex EmailRegex();

    public ValidEmailAddressAttribute() : base(DataType.EmailAddress)
    {
    }

    public override bool IsValid(object value)
    {
        // Match EmailAddressAttribute: null/empty is valid; required-ness is handled separately
        if (value is null)
        {
            return true;
        }

        if (value is not string email)
        {
            return false;
        }

        if (email.Length == 0)
        {
            return true;
        }

        return EmailRegex().IsMatch(email);
    }
}
