namespace NetEvolve.ForgingBlazor.Configurations;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

public sealed class SiteConfig
{
    [Required]
    [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "As designed.")]
    public string BaseUrl { get; set; } = default!;

    [Required]
    [DefaultValue("en-US")]
    public string LanguageCode { get; set; } = "en-US";

    [Required]
    [DefaultValue("ForgingBlazor Application")]
    [MaxLength(100)]
    public string Title { get; set; } = "ForgingBlazor Application";
}
