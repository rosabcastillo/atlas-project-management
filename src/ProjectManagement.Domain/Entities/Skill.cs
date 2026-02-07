using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Domain.Entities;

public class Skill
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ResourceSkill> ResourceSkills { get; set; } = new List<ResourceSkill>();
}
