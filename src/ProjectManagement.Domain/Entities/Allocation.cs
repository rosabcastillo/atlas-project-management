namespace ProjectManagement.Domain.Entities;

public class Allocation
{
    public int Id { get; set; }

    public int ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int? Percentage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
