using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AccountableLead { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public Priority? Priority { get; set; }
    public int? StatusId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ProjectStatus? Status { get; set; }
    public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
}
