using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class LabelWriteDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Color is required.")]
    [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex code (e.g., #RRGGBB or #RGB).")]
    public string Color { get; set; } = "#c0f9ba";
}
