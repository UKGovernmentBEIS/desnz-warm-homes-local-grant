using System.ComponentModel.DataAnnotations.Schema;

namespace HerPublicWebsite.BusinessLogic.Models;

[Table("Sessions")]
public class Session
{
    public int Id { get; set; }

    public DateTime Timestamp;

    public bool IsJourneyComplete { get; set; }
    
    //IsEligible being Null means eligibility has not been determined
    public bool? IsEligible { get; set; }
}