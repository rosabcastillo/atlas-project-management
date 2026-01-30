using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Roles.AnyAsync())
            return;

        // Seed Project Statuses
        var statuses = new List<ProjectStatus>
        {
            new() { Name = "Not Yet Started", Color = "#6b7280", DisplayOrder = 1 },  // Gray
            new() { Name = "Refinement", Color = "#8b5cf6", DisplayOrder = 2 },       // Purple
            new() { Name = "In Progress", Color = "#3b82f6", DisplayOrder = 3 },      // Blue
            new() { Name = "Ready for Pilot", Color = "#f59e0b", DisplayOrder = 4 },  // Amber
            new() { Name = "Pilot", Color = "#f97316", DisplayOrder = 5 },            // Orange
            new() { Name = "Launched", Color = "#10b981", DisplayOrder = 6 },         // Green
            new() { Name = "Support", Color = "#06b6d4", DisplayOrder = 7 },          // Cyan
            new() { Name = "On Hold", Color = "#ef4444", DisplayOrder = 8 }           // Red
        };
        await context.ProjectStatuses.AddRangeAsync(statuses);
        await context.SaveChangesAsync();

        // Seed Roles
        var roles = new List<Role>
        {
            new() { Name = "Product Owner", RequiresCapacity = false },
            new() { Name = "Scrum Master", RequiresCapacity = false },
            new() { Name = "BA", RequiresCapacity = false },
            new() { Name = "Developer", RequiresCapacity = true },
            new() { Name = "QA", RequiresCapacity = true }
        };
        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        // Seed Vendors
        var vendors = new List<Vendor>
        {
            new() { Name = "Bridgenext" },
            new() { Name = "Independent" }
        };
        await context.Vendors.AddRangeAsync(vendors);
        await context.SaveChangesAsync();

        // Seed Skills
        var skills = new List<Skill>
        {
            new() { Name = "C#" },
            new() { Name = ".NET" },
            new() { Name = "Azure" },
            new() { Name = "SQL Server" },
            new() { Name = "React" },
            new() { Name = "Flutter" },
            new() { Name = "DART" },
            new() { Name = "Python" },
            new() { Name = "Workato" },
            new() { Name = "Vapi" },
            new() { Name = "LLM" },
            new() { Name = "Mobile" },
            new() { Name = "ML" },
            new() { Name = "RPA" },
            new() { Name = "MCP" }
        };
        await context.Skills.AddRangeAsync(skills);
        await context.SaveChangesAsync();

        // Seed Projects (from the Excel sheet)
        var projects = new List<Project>
        {
            new() { Name = "Candidate and Recruiter Portal", AccountableLead = "Tarandeep", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "Oliver", AccountableLead = "Tarandeep" },
            new() { Name = "SOC Voice AI", AccountableLead = "Tarandeep" },
            new() { Name = "Allison Stability", AccountableLead = "Tarandeep" },
            new() { Name = "SOC Voice AI for Credentialing status checks", AccountableLead = "Tarandeep" },
            new() { Name = "OB & Cred RPA - SC Credentialing", AccountableLead = "Shubham", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "SC candidate portal + Credentialing CC page", AccountableLead = "Shubham", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "AR Tool", AccountableLead = "Jess", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "Payer Profile", AccountableLead = "Jess", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "RCM including Auth SOC", AccountableLead = "Jess", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "SOC - Medical Necessity and RA - 6 Month Assessment Summary", AccountableLead = "Emily" },
            new() { Name = "SOC - Marketing improve lead quality POC", AccountableLead = "Dinesh", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "Clinical Incident Ticket Roll Out - ManageEngine", AccountableLead = "Joe", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "CCS Change", AccountableLead = "Jess" },
            new() { Name = "CCS - Assessment Signature", AccountableLead = "Emily R" },
            new() { Name = "Treatment Plan not exceeding weekly auth", AccountableLead = "Jess" },
            new() { Name = "Governance", AccountableLead = "Jess", DeliveryMethod = DeliveryMethod.Scrum },
            new() { Name = "LP", AccountableLead = "Emily R", DeliveryMethod = DeliveryMethod.Scrum },
            new() { Name = "SOC - ETP, Clinical Review, Family Signatures", AccountableLead = "Emily R", DeliveryMethod = DeliveryMethod.Scrum },
            new() { Name = "SOC Acceleration - Auto-shells, IA Blocks, Auto-assign clinician", AccountableLead = "Sean", DeliveryMethod = DeliveryMethod.Scrum },
            new() { Name = "Improve Caseload AI accuracy", AccountableLead = "Sean" },
            new() { Name = "Family Connect App", AccountableLead = "Sean" },
            new() { Name = "Tech Debt - Rhapsody migration, Azure isolate Test", AccountableLead = "Rakesh", DeliveryMethod = DeliveryMethod.Kanban },
            new() { Name = "HR Various Projects", AccountableLead = "Jess" },
            new() { Name = "Critical security vulnerabilities", AccountableLead = "Haresh" },
            new() { Name = "ISBA Training assessment live", AccountableLead = "Jess" },
            new() { Name = "Joint Commission", AccountableLead = "Emily" },
            new() { Name = "MCP Foundation & Other AI priorities", AccountableLead = "Jay" },
            new() { Name = "1.0 to 2.0 Migration", AccountableLead = "Rosa", DeliveryMethod = DeliveryMethod.Scrum }
        };
        await context.Projects.AddRangeAsync(projects);
        await context.SaveChangesAsync();

        // Seed Resources (from the Excel sheet)
        var bridgenext = vendors.First(v => v.Name == "Bridgenext");
        var independent = vendors.First(v => v.Name == "Independent");
        var poRole = roles.First(r => r.Name == "Product Owner");
        var smRole = roles.First(r => r.Name == "Scrum Master");
        var baRole = roles.First(r => r.Name == "BA");
        var devRole = roles.First(r => r.Name == "Developer");

        var resources = new List<Resource>
        {
            // Scrum Masters
            new() { Name = "Rosa", RoleId = smRole.Id },
            new() { Name = "Tarandeep", RoleId = smRole.Id },
            new() { Name = "Joe", RoleId = smRole.Id },

            // Product Owners
            new() { Name = "Jess P", RoleId = poRole.Id },
            new() { Name = "Emily R", RoleId = poRole.Id },
            new() { Name = "Sean D", RoleId = poRole.Id },

            // BAs
            new() { Name = "Shubham Sarathe", RoleId = baRole.Id },
            new() { Name = "JP", RoleId = baRole.Id },
            new() { Name = "Annie", RoleId = baRole.Id },
            new() { Name = "Peggy", RoleId = baRole.Id },
            new() { Name = "Swamy Bommidi", RoleId = baRole.Id, VendorId = bridgenext.Id },
            new() { Name = "Haritha", RoleId = baRole.Id },
            new() { Name = "Simran", RoleId = baRole.Id },
            new() { Name = "Jakob", RoleId = baRole.Id, VendorId = independent.Id },
            new() { Name = "Emma", RoleId = baRole.Id },

            // Developers
            new() { Name = "Harsh Darji", RoleId = devRole.Id, VendorId = bridgenext.Id },
            new() { Name = "Priyanka Gat", RoleId = devRole.Id, VendorId = bridgenext.Id },
            new() { Name = "Priyanka Tekale", RoleId = devRole.Id, VendorId = bridgenext.Id },
            new() { Name = "Shweta Dabholkar", RoleId = devRole.Id, VendorId = bridgenext.Id },
            new() { Name = "Suyog Arde", RoleId = devRole.Id, VendorId = bridgenext.Id }
        };
        await context.Resources.AddRangeAsync(resources);
        await context.SaveChangesAsync();

        // Assign skills to developers
        var csharp = skills.First(s => s.Name == "C#");
        var dotnet = skills.First(s => s.Name == ".NET");
        var azure = skills.First(s => s.Name == "Azure");
        var sqlServer = skills.First(s => s.Name == "SQL Server");
        var react = skills.First(s => s.Name == "React");

        var devResources = resources.Where(r => r.RoleId == devRole.Id).ToList();
        var resourceSkills = new List<ResourceSkill>();

        foreach (var dev in devResources)
        {
            resourceSkills.Add(new ResourceSkill { ResourceId = dev.Id, SkillId = csharp.Id });
            resourceSkills.Add(new ResourceSkill { ResourceId = dev.Id, SkillId = dotnet.Id });
            resourceSkills.Add(new ResourceSkill { ResourceId = dev.Id, SkillId = sqlServer.Id });
            resourceSkills.Add(new ResourceSkill { ResourceId = dev.Id, SkillId = react.Id });
        }

        // Add Azure skill to Harsh Darji
        var harshDarji = devResources.First(r => r.Name == "Harsh Darji");
        resourceSkills.Add(new ResourceSkill { ResourceId = harshDarji.Id, SkillId = azure.Id });

        await context.ResourceSkills.AddRangeAsync(resourceSkills);
        await context.SaveChangesAsync();

        // Seed Periods (Q1 FY26)
        var periods = new List<Period>
        {
            new() { Name = "January 2026", Type = PeriodType.Month, StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 1, 31) },
            new() { Name = "February 2026", Type = PeriodType.Month, StartDate = new DateTime(2026, 2, 1), EndDate = new DateTime(2026, 2, 28) },
            new() { Name = "March 2026", Type = PeriodType.Month, StartDate = new DateTime(2026, 3, 1), EndDate = new DateTime(2026, 3, 31) },
            new() { Name = "Q1 FY26", Type = PeriodType.Quarter, StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 3, 31) },
            new() { Name = "Q2 FY26", Type = PeriodType.Quarter, StartDate = new DateTime(2026, 4, 1), EndDate = new DateTime(2026, 6, 30) }
        };
        await context.Periods.AddRangeAsync(periods);
        await context.SaveChangesAsync();

        // Seed Allocations
        var jan2026 = periods.First(p => p.Name == "January 2026");
        var feb2026 = periods.First(p => p.Name == "February 2026");
        var qaTole = roles.First(r => r.Name == "QA");

        var candidatePortal = projects.First(p => p.Name == "Candidate and Recruiter Portal");
        var oliver = projects.First(p => p.Name == "Oliver");
        var socVoiceAI = projects.First(p => p.Name == "SOC Voice AI");
        var arTool = projects.First(p => p.Name == "AR Tool");
        var governance = projects.First(p => p.Name == "Governance");
        var lp = projects.First(p => p.Name == "LP");
        var migration = projects.First(p => p.Name == "1.0 to 2.0 Migration");
        var familyConnect = projects.First(p => p.Name == "Family Connect App");

        var harsh = resources.First(r => r.Name == "Harsh Darji");
        var priyankaG = resources.First(r => r.Name == "Priyanka Gat");
        var priyankaT = resources.First(r => r.Name == "Priyanka Tekale");
        var shweta = resources.First(r => r.Name == "Shweta Dabholkar");
        var suyog = resources.First(r => r.Name == "Suyog Arde");
        var rosa = resources.First(r => r.Name == "Rosa");
        var tarandeep = resources.First(r => r.Name == "Tarandeep");
        var jessP = resources.First(r => r.Name == "Jess P");
        var shubham = resources.First(r => r.Name == "Shubham Sarathe");

        var allocations = new List<Allocation>
        {
            // Harsh Darji - January
            new() { ResourceId = harsh.Id, ProjectId = candidatePortal.Id, PeriodId = jan2026.Id, Percentage = 50 },
            new() { ResourceId = harsh.Id, ProjectId = oliver.Id, PeriodId = jan2026.Id, Percentage = 30 },
            new() { ResourceId = harsh.Id, ProjectId = socVoiceAI.Id, PeriodId = jan2026.Id, Percentage = 20 },

            // Priyanka Gat - January
            new() { ResourceId = priyankaG.Id, ProjectId = arTool.Id, PeriodId = jan2026.Id, Percentage = 60 },
            new() { ResourceId = priyankaG.Id, ProjectId = governance.Id, PeriodId = jan2026.Id, Percentage = 40 },

            // Priyanka Tekale - January
            new() { ResourceId = priyankaT.Id, ProjectId = lp.Id, PeriodId = jan2026.Id, Percentage = 70 },
            new() { ResourceId = priyankaT.Id, ProjectId = migration.Id, PeriodId = jan2026.Id, Percentage = 30 },

            // Shweta - January
            new() { ResourceId = shweta.Id, ProjectId = familyConnect.Id, PeriodId = jan2026.Id, Percentage = 50 },
            new() { ResourceId = shweta.Id, ProjectId = candidatePortal.Id, PeriodId = jan2026.Id, Percentage = 50 },

            // Suyog - January
            new() { ResourceId = suyog.Id, ProjectId = migration.Id, PeriodId = jan2026.Id, Percentage = 80 },
            new() { ResourceId = suyog.Id, ProjectId = socVoiceAI.Id, PeriodId = jan2026.Id, Percentage = 20 },

            // Rosa (SM) - January
            new() { ResourceId = rosa.Id, ProjectId = migration.Id, PeriodId = jan2026.Id, Percentage = null },
            new() { ResourceId = rosa.Id, ProjectId = governance.Id, PeriodId = jan2026.Id, Percentage = null },

            // Tarandeep (SM) - January
            new() { ResourceId = tarandeep.Id, ProjectId = candidatePortal.Id, PeriodId = jan2026.Id, Percentage = null },
            new() { ResourceId = tarandeep.Id, ProjectId = oliver.Id, PeriodId = jan2026.Id, Percentage = null },

            // Jess P (PO) - January
            new() { ResourceId = jessP.Id, ProjectId = arTool.Id, PeriodId = jan2026.Id, Percentage = null },
            new() { ResourceId = jessP.Id, ProjectId = governance.Id, PeriodId = jan2026.Id, Percentage = null },

            // Shubham (BA) - January
            new() { ResourceId = shubham.Id, ProjectId = candidatePortal.Id, PeriodId = jan2026.Id, Percentage = null },
            new() { ResourceId = shubham.Id, ProjectId = socVoiceAI.Id, PeriodId = jan2026.Id, Percentage = null },

            // February allocations
            new() { ResourceId = harsh.Id, ProjectId = candidatePortal.Id, PeriodId = feb2026.Id, Percentage = 40 },
            new() { ResourceId = harsh.Id, ProjectId = familyConnect.Id, PeriodId = feb2026.Id, Percentage = 60 },

            new() { ResourceId = priyankaG.Id, ProjectId = arTool.Id, PeriodId = feb2026.Id, Percentage = 50 },
            new() { ResourceId = priyankaG.Id, ProjectId = lp.Id, PeriodId = feb2026.Id, Percentage = 50 },

            new() { ResourceId = suyog.Id, ProjectId = migration.Id, PeriodId = feb2026.Id, Percentage = 100 },
        };
        await context.Allocations.AddRangeAsync(allocations);
        await context.SaveChangesAsync();
    }
}
