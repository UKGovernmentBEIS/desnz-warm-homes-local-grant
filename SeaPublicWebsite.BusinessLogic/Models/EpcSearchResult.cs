using System.Globalization;
using System.Text.RegularExpressions;

namespace SeaPublicWebsite.BusinessLogic.Models
{
    public class EpcSearchResult
    {
        public string EpcId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }
        public string Postcode { get; set; }

        public EpcSearchResult(string epcId, string address1, string address2, string postcode)
        {
            EpcId = epcId;
            Address1 = address1;
            Address2 = address2;
            Postcode = postcode;
            FixFormatting();
        }

        private void FixFormatting()
        {
            var tI = new CultureInfo("en-GB", false).TextInfo;
            Address1 = tI.ToTitleCase(Address1.ToLower().Replace(",", ""));
            Address2 = tI.ToTitleCase(Address2.ToLower().Replace(",", ""));
        }

        public static List<EpcSearchResult> SortEpcsInformation(List<EpcSearchResult> epcsInformation)
        {
            epcsInformation.Sort(SortEpcsInformationByHouseNumberOrAlphabetically);
            return epcsInformation;
        }
        
        private static int SortEpcsInformationByHouseNumberOrAlphabetically(EpcSearchResult epcInformation1, EpcSearchResult epcInformation2)
        {
            var x = (epcInformation1.GetHouseNumber(), epcInformation2.GetHouseNumber()) switch
            {
                (null, null) => SortEpcsInformationAlphabetically(epcInformation1, epcInformation2),
                (null, _) => 1,
                (_, null) => -1,
                var (houseNumber1, houseNumber2) => (houseNumber1.Value - houseNumber2.Value) switch
                {
                    <0 => -1,
                    >0 => 1,
                    _ => SortEpcsInformationAlphabetically(epcInformation1, epcInformation2)
                }
            };
            return x;
        }

        private static int SortEpcsInformationAlphabetically(EpcSearchResult epcInformation1, EpcSearchResult epcInformation2)
        {
            var comparedAddress1 = string.Compare(epcInformation1.Address1, epcInformation2.Address1,
                StringComparison.OrdinalIgnoreCase);
            var comparedAddress2 = string.Compare(epcInformation1.Address2, epcInformation2.Address2,
                StringComparison.OrdinalIgnoreCase);
            return comparedAddress1 != 0 ? comparedAddress1 : comparedAddress2;
        }
        
        
        private int? GetHouseNumber() {
            var houseNumberFromFirstLine = GetIntegerFromStartOfString(Address1);
            var houseNumberFromSecondLine = GetIntegerFromStartOfString(Address2);
            return houseNumberFromFirstLine ?? houseNumberFromSecondLine;
        }

        private int? GetIntegerFromStartOfString(string input) {
            var regex = new Regex("^[0-9]+", RegexOptions.IgnoreCase);
            var regexMatches = regex.Match(input);

            var number = 0;
            var result = false;

            if (regexMatches.Success)
            {
                var numberAsString = regexMatches.Groups[0].Value;
                result = int.TryParse(numberAsString, out number);
            }

            return result ? number : null;
        }
    }
}

