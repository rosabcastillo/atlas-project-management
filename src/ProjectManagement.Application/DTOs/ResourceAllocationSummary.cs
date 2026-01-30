namespace ProjectManagement.Application.DTOs;

public class ResourceAllocationSummary
{
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool RequiresCapacity { get; set; }
    public int PeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public int TotalPercentage { get; set; }
    public bool IsOverAllocated => RequiresCapacity && TotalPercentage > 100;
    public List<ProjectAllocation> ProjectAllocations { get; set; } = new();
}

public class ProjectAllocation
{
    public int AllocationId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? Percentage { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
