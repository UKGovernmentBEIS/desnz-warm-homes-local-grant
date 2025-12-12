using System.ComponentModel.DataAnnotations.Schema;

namespace WhlgPublicWebsite.BusinessLogic.Models;

public enum EmergencyMaintenanceState
{
    Enabled,
    Disabled
}

[Table("EmergencyMaintenanceHistories")]
public class EmergencyMaintenanceHistory : IEntityWithRowVersioning
{
    public uint Version { get; set; }

    public int Id { get; set; }

    public EmergencyMaintenanceState State { get; set; }

    public DateTime ChangeDate { get; set; }

    public string AuthorEmail { get; set; }

    public EmergencyMaintenanceHistory()
    {
    }
}