namespace ProjectManagement.Domain.Entities;

public class Theme
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
