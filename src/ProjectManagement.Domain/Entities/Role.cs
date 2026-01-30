namespace ProjectManagement.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool RequiresCapacity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
