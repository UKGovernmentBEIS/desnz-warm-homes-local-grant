using System.ComponentModel.DataAnnotations.Schema;

namespace WhlgPublicWebsite.BusinessLogic.Models;

[Table("Sessions")]
public class Session : IEntityWithRowVersioning
{
    public int Id { get; set; }

    public DateTime Timestamp;

    public bool IsJourneyComplete { get; set; }

    /* While IsEligible == null, the user's eligibility has not been determined
     * IsEligible should therefore always be null while IsJourneyComplete is false
     * If IsJourneyComplete is true, the user's LA is Not Participating/Not Taking Part, so eligibility was not calculated
     */
    public bool? IsEligible { get; set; }

    public uint Version { get; set; }
}