using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper;

namespace HerPublicWebsite.BusinessLogic.Models;

[Table("Sessions")]
public class Session
{
    public int Id { get; set; }

    public DateTime Timestamp;

    public bool IsJourneyComplete { get; set; }
}