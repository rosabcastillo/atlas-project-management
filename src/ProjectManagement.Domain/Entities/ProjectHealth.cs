namespace ProjectManagement.Domain.Entities;

public class ProjectHealth : OrderedLookupEntity
{
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
