using System.Text.RegularExpressions;

namespace HerPublicWebsite.BusinessLogic.Extensions;

public static class StringExtensions
{
    private static Regex[] validPostCodeRegexes = new[]
    {
        new Regex(@"^[A-Z]\d \d[A-Z][A-Z]$", RegexOptions.Compiled),          //   AN NAA
        new Regex(@"^[A-Z]\d\d \d[A-Z][A-Z]$", RegexOptions.Compiled),        //  ANN NAA
        new Regex(@"^[A-Z][A-Z]\d \d[A-Z][A-Z]$", RegexOptions.Compiled),     //  AAN NAA
        new Regex(@"^[A-Z][A-Z]\d\d \d[A-Z][A-Z]$", RegexOptions.Compiled),   // AANN NAA
        new Regex(@"^[A-Z]\d[A-Z] \d[A-Z][A-Z]$", RegexOptions.Compiled),     //  ANA NAA
        new Regex(@"^[A-Z][A-Z]\d[A-Z] \d[A-Z][A-Z]$", RegexOptions.Compiled) // AANA NAA
    };

    private static Regex multipleSpacesRegex = new Regex(@"[ ]{2,}", RegexOptions.Compiled);

    // Returns true if a string is in the correct format to be a postcode.
    // This method does not check whether the actual postcode exists
    public static bool IsValidUkPostcodeFormat(this string value)
    {
        var interimValue = NormaliseToUkPostcodeFormatInternal(value);
        return validPostCodeRegexes.Any(r => r.IsMatch(interimValue));
    }

    // Normalises a postcode to upper case with correct spacing. Returns null if the string cannot be a UK postcode
    public static string NormaliseToUkPostcodeFormat(this string value)
    {
        var interimValue = NormaliseToUkPostcodeFormatInternal(value);
        return validPostCodeRegexes.Any(r => r.IsMatch(interimValue)) ? interimValue : null;
    }

    private static string NormaliseToUkPostcodeFormatInternal(string value)
    {
        // Remove leading and trailing whitespace
        var interimValue = value.Trim();

        interimValue = interimValue.ToUpperInvariant();
        
        // Replace multiple consecutive spaces with a single space
        interimValue = multipleSpacesRegex.Replace(interimValue, " ");

        // All postcodes end in a number and two letters (the inward postcode)
        // Insert a space before the inward postcode if it's not already there.
        if (interimValue.Length >= 4 && interimValue[^4] != ' ')
        {
            interimValue = interimValue.Substring(0, interimValue.Length - 3) + " " +
                           interimValue.Substring(interimValue.Length - 3);
        }

        return interimValue;
    }
}