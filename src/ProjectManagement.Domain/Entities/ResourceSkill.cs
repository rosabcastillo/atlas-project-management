namespace ProjectManagement.Domain.Entities;

public class ResourceSkill
{
    public int ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;

    public int SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}
