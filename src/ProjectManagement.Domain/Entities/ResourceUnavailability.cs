using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class ResourceUnavailability
{
    public int Id { get; set; }

    public int ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int Hours { get; set; } = 8;

    public UnavailabilityReason Reason { get; set; } = UnavailabilityReason.PTO;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
