namespace ProjectManagement.Domain.Entities;

public class Vendor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
