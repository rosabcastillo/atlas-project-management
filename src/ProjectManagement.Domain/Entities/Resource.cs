using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Domain.Entities;

public class Resource
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public int? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ResourceRole> ResourceRoles { get; set; } = new List<ResourceRole>();
    public ICollection<ResourceSkill> ResourceSkills { get; set; } = new List<ResourceSkill>();
    public ICollection<Allocation> Allocations { get; set; } = new List<Allocation>();
    public ICollection<ResourceUnavailability> Unavailabilities { get; set; } = new List<ResourceUnavailability>();
}
