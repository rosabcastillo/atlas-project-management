namespace ProjectManagement.Domain.Entities;

public class ResourceRole
{
    public int ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
