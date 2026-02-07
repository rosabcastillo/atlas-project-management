namespace ProjectManagement.Domain.Entities;

public class ProjectStatus : OrderedLookupEntity
{
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
