namespace ProjectManagement.Application.DTOs;

public class AllocationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int? AllocationId { get; set; }
    public int TotalAllocationPercentage { get; set; }

    public static AllocationResult Ok(int allocationId, int totalPercentage) => new()
    {
        Success = true,
        AllocationId = allocationId,
        TotalAllocationPercentage = totalPercentage
    };

    public static AllocationResult Error(string message, int totalPercentage) => new()
    {
        Success = false,
        ErrorMessage = message,
        TotalAllocationPercentage = totalPercentage
    };
}
