using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Domain.Entities;

public abstract class OrderedLookupEntity
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(7)]
    public string Color { get; set; } = "#6b7280"; // Default gray

    public int DisplayOrder { get; set; }

    [MaxLength(10)]
    public string? ShortName { get; set; }
}
