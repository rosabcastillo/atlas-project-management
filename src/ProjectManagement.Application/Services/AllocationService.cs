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
        int periodId,
        int? percentage)
    {
        var resource = await _resourceRepository.Query()
            .Include(r => r.Role)
            .FirstOrDefaultAsync(r => r.Id == resourceId);

        if (resource == null)
            return AllocationResult.Error("Resource not found", 0);

        var currentTotal = await GetTotalAllocationForPeriodAsync(resourceId, periodId);
        var newPercentage = percentage ?? 0;

        if (resource.Role.RequiresCapacity)
        {
            if (!percentage.HasValue)
                return AllocationResult.Error("Percentage is required for this role", currentTotal);

            if (currentTotal + newPercentage > 100)
            {
                return AllocationResult.Error(
                    $"Cannot allocate {newPercentage}%. Resource is already at {currentTotal}% allocation. Maximum available: {100 - currentTotal}%",
                    currentTotal);
            }
        }

        var allocation = new Allocation
        {
            ResourceId = resourceId,
            ProjectId = projectId,
            PeriodId = periodId,
            Percentage = percentage
        };

        var created = await _allocationRepository.AddAsync(allocation);
        return AllocationResult.Ok(created.Id, currentTotal + newPercentage);
    }

    public async Task<AllocationResult> UpdateAllocationAsync(
        int allocationId,
        int? newPercentage)
    {
        var allocation = await _allocationRepository.Query()
            .Include(a => a.Resource).ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(a => a.Id == allocationId);

        if (allocation == null)
            return AllocationResult.Error("Allocation not found", 0);

        var currentTotal = await GetTotalAllocationForPeriodAsync(
            allocation.ResourceId,
            allocation.PeriodId,
            excludeAllocationId: allocationId);

        var newPercentageValue = newPercentage ?? 0;

        if (allocation.Resource.Role.RequiresCapacity)
        {
            if (!newPercentage.HasValue)
                return AllocationResult.Error("Percentage is required for this role", currentTotal);

            if (currentTotal + newPercentageValue > 100)
            {
                return AllocationResult.Error(
                    $"Cannot update to {newPercentageValue}%. Other allocations total {currentTotal}%. Maximum available: {100 - currentTotal}%",
                    currentTotal);
            }
        }

        allocation.Percentage = newPercentage;
        await _allocationRepository.UpdateAsync(allocation);

        return AllocationResult.Ok(allocation.Id, currentTotal + newPercentageValue);
    }

    public async Task<int> GetTotalAllocationForPeriodAsync(
        int resourceId,
        int periodId,
        int? excludeAllocationId = null)
    {
        var query = _allocationRepository.Query()
            .Include(a => a.Resource).ThenInclude(r => r.Role)
            .Where(a => a.ResourceId == resourceId &&
                       a.PeriodId == periodId &&
                       a.Resource.Role.RequiresCapacity &&
                       a.Percentage.HasValue);

        if (excludeAllocationId.HasValue)
            query = query.Where(a => a.Id != excludeAllocationId.Value);

        return await query.SumAsync(a => a.Percentage ?? 0);
    }

    public async Task<ResourceAllocationSummary> GetResourceAllocationSummaryAsync(
        int resourceId,
        int periodId)
    {
        var resource = await _resourceRepository.Query()
            .Include(r => r.Role)
            .FirstOrDefaultAsync(r => r.Id == resourceId);

        if (resource == null)
            return new ResourceAllocationSummary();

        var allocations = await _allocationRepository.Query()
            .Include(a => a.Project)
            .Include(a => a.Resource).ThenInclude(r => r.Role)
            .Include(a => a.Period)
            .Where(a => a.ResourceId == resourceId && a.PeriodId == periodId)
            .ToListAsync();

        var summary = new ResourceAllocationSummary
        {
            ResourceId = resourceId,
            ResourceName = resource.Name,
            RoleName = resource.Role.Name,
            RequiresCapacity = resource.Role.RequiresCapacity,
            PeriodId = periodId,
            PeriodName = allocations.FirstOrDefault()?.Period?.Name ?? "",
            TotalPercentage = allocations
                .Where(a => a.Resource.Role.RequiresCapacity && a.Percentage.HasValue)
                .Sum(a => a.Percentage ?? 0),
            ProjectAllocations = allocations.Select(a => new ProjectAllocation
            {
                AllocationId = a.Id,
                ProjectId = a.ProjectId,
                ProjectName = a.Project.Name,
                Percentage = a.Percentage,
                RoleName = a.Resource.Role.Name
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
}
