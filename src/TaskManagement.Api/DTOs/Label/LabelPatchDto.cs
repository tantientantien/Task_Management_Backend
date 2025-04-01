using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

public class LabelPatchDto
{
    [AllowNull]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [AllowNull]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex code (e.g., #RRGGBB or #RGB).")]
    public string Color { get; set; } = "#c0f9ba";
}
