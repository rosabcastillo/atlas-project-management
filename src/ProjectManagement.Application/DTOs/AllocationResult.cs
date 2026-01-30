namespace ProjectManagement.Application.DTOs;

public class AllocationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? AllocationId { get; set; }
    public int TotalAllocationPercentage { get; set; }
    public List<OverlapPeriod>? OverlappingPeriods { get; set; }

    public static AllocationResult Ok(int allocationId, int totalPercentage, List<OverlapPeriod>? overlaps = null) => new()
    {
        Success = true,
        AllocationId = allocationId,
        TotalAllocationPercentage = totalPercentage,
        OverlappingPeriods = overlaps
    };

    public static AllocationResult Error(string message, int totalPercentage, List<OverlapPeriod>? overlaps) => new()
    {
        Success = false,
        ErrorMessage = message,
        TotalAllocationPercentage = totalPercentage,
        OverlappingPeriods = overlaps
    };
}

public class OverlapPeriod
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalPercentage { get; set; }
    public List<string> ProjectNames { get; set; } = new();
}
