namespace ProjectManagement.Domain.Entities;

public class ProjectStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6b7280"; // Default gray
    public int DisplayOrder { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
