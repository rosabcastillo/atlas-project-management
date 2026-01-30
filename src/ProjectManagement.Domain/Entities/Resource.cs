namespace ProjectManagement.Domain.Entities;

public class Resource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ResourceSkill> ResourceSkills { get; set; } = new List<ResourceSkill>();
    public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
}
