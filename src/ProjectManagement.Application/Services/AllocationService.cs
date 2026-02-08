using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Services;

public class AllocationService
{
    private readonly IRepository<Allocation> _allocationRepository;
    private readonly IRepository<Resource> _resourceRepository;

    public AllocationService(
        IRepository<Allocation> allocationRepository,
        IRepository<Resource> resourceRepository)
    {
        _allocationRepository = allocationRepository;
        _resourceRepository = resourceRepository;
    }

    public async Task<AllocationResult> CreateAllocationAsync(
        int resourceId,
        int projectId,
        DateTime startDate,
        DateTime endDate,
        int? percentage)
    {
        var resource = await _resourceRepository.Query()
            .Include(r => r.ResourceRoles).ThenInclude(rr => rr.Role)
            .FirstOrDefaultAsync(r => r.Id == resourceId);

        if (resource == null)
            return AllocationResult.Error("Resource not found", 0, null);

        // Validate date range
        if (endDate < startDate)
            return AllocationResult.Error("End date must be after start date", 0, null);

        // Check if resource is terminated before the allocation period
        if (resource.EndDate.HasValue && startDate > resource.EndDate.Value)
            return AllocationResult.Error($"Cannot allocate. Resource contract ended on {resource.EndDate.Value:MMM dd, yyyy}", 0, null);

        // Check for overlapping allocations and capacity
        var overlappingAllocations = await GetOverlappingAllocationsAsync(resourceId, startDate, endDate);
        var newPercentage = percentage ?? 0;

        if (resource.ResourceRoles.Any(rr => rr.Role.RequiresCapacity))
        {
            if (!percentage.HasValue)
                return AllocationResult.Error("Percentage is required for this role", 0, null);

            // Calculate max overlap percentage in any overlapping period
            var overlapAnalysis = AnalyzeCapacityOverlap(overlappingAllocations, startDate, endDate, newPercentage);

            if (overlapAnalysis.MaxTotalPercentage > 100)
            {
                return AllocationResult.Error(
                    $"Cannot allocate {newPercentage}%. Resource would be at {overlapAnalysis.MaxTotalPercentage}% during overlapping period ({overlapAnalysis.OverlapStart:MMM dd} - {overlapAnalysis.OverlapEnd:MMM dd})",
                    overlapAnalysis.MaxTotalPercentage - newPercentage,
                    overlapAnalysis.OverlappingPeriods);
            }
        }

        var allocation = new Allocation
        {
            ResourceId = resourceId,
            ProjectId = projectId,
            StartDate = startDate,
            EndDate = endDate,
            Percentage = percentage
        };

        var created = await _allocationRepository.AddAsync(allocation);

        // Return any overlap warnings even if under capacity
        var warnings = overlappingAllocations.Any()
            ? GetOverlapWarnings(overlappingAllocations, startDate, endDate)
            : null;

        return AllocationResult.Ok(created.Id, newPercentage, warnings);
    }

    public async Task<AllocationResult> UpdateAllocationAsync(
        int allocationId,
        DateTime? newStartDate,
        DateTime? newEndDate,
        int? newPercentage)
    {
        var allocation = await _allocationRepository.Query()
            .Include(a => a.Resource).ThenInclude(r => r.ResourceRoles).ThenInclude(rr => rr.Role)
            .FirstOrDefaultAsync(a => a.Id == allocationId);

        if (allocation == null)
            return AllocationResult.Error("Allocation not found", 0, null);

        var startDate = newStartDate ?? allocation.StartDate;
        var endDate = newEndDate ?? allocation.EndDate;
        var percentage = newPercentage ?? allocation.Percentage;

        // Validate date range
        if (endDate < startDate)
            return AllocationResult.Error("End date must be after start date", 0, null);

        // Check resource termination
        if (allocation.Resource.EndDate.HasValue && startDate > allocation.Resource.EndDate.Value)
            return AllocationResult.Error($"Cannot allocate. Resource contract ended on {allocation.Resource.EndDate.Value:MMM dd, yyyy}", 0, null);

        var overlappingAllocations = await GetOverlappingAllocationsAsync(
            allocation.ResourceId, startDate, endDate, excludeAllocationId: allocationId);

        var newPercentageValue = percentage ?? 0;

        if (allocation.Resource.ResourceRoles.Any(rr => rr.Role.RequiresCapacity))
        {
            if (!percentage.HasValue)
                return AllocationResult.Error("Percentage is required for this role", 0, null);

            var overlapAnalysis = AnalyzeCapacityOverlap(overlappingAllocations, startDate, endDate, newPercentageValue);

            if (overlapAnalysis.MaxTotalPercentage > 100)
            {
                return AllocationResult.Error(
                    $"Cannot update to {newPercentageValue}%. Resource would be at {overlapAnalysis.MaxTotalPercentage}% during overlapping period",
                    overlapAnalysis.MaxTotalPercentage - newPercentageValue,
                    overlapAnalysis.OverlappingPeriods);
            }
        }

        allocation.StartDate = startDate;
        allocation.EndDate = endDate;
        allocation.Percentage = percentage;
        await _allocationRepository.UpdateAsync(allocation);

        var warnings = overlappingAllocations.Any()
            ? GetOverlapWarnings(overlappingAllocations, startDate, endDate)
            : null;

        return AllocationResult.Ok(allocation.Id, newPercentageValue, warnings);
    }

    public async Task<List<Allocation>> GetOverlappingAllocationsAsync(
        int resourceId,
        DateTime startDate,
        DateTime endDate,
        int? excludeAllocationId = null)
    {
        var query = _allocationRepository.Query()
            .Include(a => a.Project)
            .Include(a => a.Resource).ThenInclude(r => r.ResourceRoles).ThenInclude(rr => rr.Role)
            .Where(a => a.ResourceId == resourceId &&
                       a.StartDate <= endDate &&
                       a.EndDate >= startDate);

        if (excludeAllocationId.HasValue)
            query = query.Where(a => a.Id != excludeAllocationId.Value);

        return await query.ToListAsync();
    }

    private CapacityOverlapAnalysis AnalyzeCapacityOverlap(
        List<Allocation> overlappingAllocations,
        DateTime newStart,
        DateTime newEnd,
        int newPercentage)
    {
        if (!overlappingAllocations.Any())
        {
            return new CapacityOverlapAnalysis
            {
                MaxTotalPercentage = newPercentage,
                OverlapStart = newStart,
                OverlapEnd = newEnd
            };
        }

        // Find all date boundaries to analyze each distinct period
        var boundaries = new HashSet<DateTime> { newStart, newEnd };
        foreach (var alloc in overlappingAllocations)
        {
            if (alloc.StartDate >= newStart && alloc.StartDate <= newEnd)
                boundaries.Add(alloc.StartDate);
            if (alloc.EndDate >= newStart && alloc.EndDate <= newEnd)
                boundaries.Add(alloc.EndDate);
        }

        var sortedBoundaries = boundaries.OrderBy(d => d).ToList();

        int maxTotal = newPercentage;
        DateTime maxStart = newStart;
        DateTime maxEnd = newEnd;
        var overlappingPeriods = new List<OverlapPeriod>();

        // Analyze each period between boundaries
        for (int i = 0; i < sortedBoundaries.Count - 1; i++)
        {
            var periodStart = sortedBoundaries[i];
            var periodEnd = sortedBoundaries[i + 1];
            var midPoint = periodStart.AddDays((periodEnd - periodStart).TotalDays / 2);

            var periodTotal = newPercentage + overlappingAllocations
                .Where(a => a.StartDate <= midPoint && a.EndDate >= midPoint && a.Percentage.HasValue)
                .Sum(a => a.Percentage ?? 0);

            if (periodTotal > maxTotal)
            {
                maxTotal = periodTotal;
                maxStart = periodStart;
                maxEnd = periodEnd;
            }

            if (periodTotal > 0)
            {
                var projectsInPeriod = overlappingAllocations
                    .Where(a => a.StartDate <= midPoint && a.EndDate >= midPoint)
                    .Select(a => a.Project.Name)
                    .ToList();

                if (projectsInPeriod.Any())
                {
                    overlappingPeriods.Add(new OverlapPeriod
                    {
                        StartDate = periodStart,
                        EndDate = periodEnd,
                        TotalPercentage = periodTotal,
                        ProjectNames = projectsInPeriod
                    });
                }
            }
        }

        return new CapacityOverlapAnalysis
        {
            MaxTotalPercentage = maxTotal,
            OverlapStart = maxStart,
            OverlapEnd = maxEnd,
            OverlappingPeriods = overlappingPeriods
        };
    }

    private List<OverlapPeriod> GetOverlapWarnings(
        List<Allocation> overlappingAllocations,
        DateTime newStart,
        DateTime newEnd)
    {
        return overlappingAllocations
            .Select(a => new OverlapPeriod
            {
                StartDate = a.StartDate > newStart ? a.StartDate : newStart,
                EndDate = a.EndDate < newEnd ? a.EndDate : newEnd,
                TotalPercentage = a.Percentage ?? 0,
                ProjectNames = new List<string> { a.Project.Name }
            })
            .ToList();
    }

    public async Task<ResourceAllocationSummary> GetResourceAllocationSummaryAsync(
        int resourceId,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var resource = await _resourceRepository.Query()
            .Include(r => r.ResourceRoles).ThenInclude(rr => rr.Role)
            .FirstOrDefaultAsync(r => r.Id == resourceId);

        if (resource == null)
            return new ResourceAllocationSummary();

        var query = _allocationRepository.Query()
            .Include(a => a.Project)
            .Include(a => a.Resource).ThenInclude(r => r.ResourceRoles).ThenInclude(rr => rr.Role)
            .Where(a => a.ResourceId == resourceId);

        if (startDate.HasValue && endDate.HasValue)
        {
            // Get allocations that overlap with the specified date range
            query = query.Where(a => a.StartDate <= endDate.Value && a.EndDate >= startDate.Value);
        }

        var allocations = await query.ToListAsync();

        var summary = new ResourceAllocationSummary
        {
            ResourceId = resourceId,
            ResourceName = resource.Name,
            RoleName = string.Join(", ", resource.ResourceRoles.Select(rr => rr.Role.Name)),
            RequiresCapacity = resource.ResourceRoles.Any(rr => rr.Role.RequiresCapacity),
            StartDate = startDate,
            EndDate = endDate,
            TotalPercentage = allocations
                .Where(a => a.Resource.ResourceRoles.Any(rr => rr.Role.RequiresCapacity) && a.Percentage.HasValue)
                .Sum(a => a.Percentage ?? 0),
            ProjectAllocations = allocations.Select(a => new ProjectAllocation
            {
                AllocationId = a.Id,
                ProjectId = a.ProjectId,
                ProjectName = a.Project.Name,
                Percentage = a.Percentage,
                RoleName = string.Join(", ", a.Resource.ResourceRoles.Select(rr => rr.Role.Name)),
                StartDate = a.StartDate,
                EndDate = a.EndDate
            }).ToList()
        };

        return summary;
    }

    public async Task DeleteAllocationAsync(int allocationId)
    {
        var allocation = await _allocationRepository.GetByIdAsync(allocationId);
        if (allocation != null)
            await _allocationRepository.DeleteAsync(allocation);
    }

    private class CapacityOverlapAnalysis
    {
        public int MaxTotalPercentage { get; set; }
        public DateTime OverlapStart { get; set; }
        public DateTime OverlapEnd { get; set; }
        public List<OverlapPeriod> OverlappingPeriods { get; set; } = new();
    }
}
