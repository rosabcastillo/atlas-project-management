using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Domain.Entities;

public class Period
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PeriodType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
}
