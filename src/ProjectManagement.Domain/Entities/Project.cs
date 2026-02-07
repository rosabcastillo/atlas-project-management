using System.ComponentModel.DataAnnotations;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class Project
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AccountableLead { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public Priority? Priority { get; set; }
    public int? StatusId { get; set; }
    public int? ThemeId { get; set; }
    public int? HealthId { get; set; }
    public DateTime? TargetPilotGoLiveDate { get; set; }
    public DateTime? TargetEnterpriseGoLiveDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ProjectStatus? Status { get; set; }
    public Theme? Theme { get; set; }
    public ProjectHealth? Health { get; set; }
    public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
}
