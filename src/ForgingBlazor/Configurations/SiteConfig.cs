namespace NetEvolve.ForgingBlazor.Configurations;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the configuration values required to describe the site.
/// </summary>
public sealed class SiteConfig
{
    /// <summary>
    /// Gets or sets the base URL of the site as an absolute URI.
    /// </summary>
    [Required]
    [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "As designed.")]
    public string BaseUrl { get; set; } = default!;

    /// <summary>
    /// Gets or sets the language code for the site content.
    /// </summary>
    [Required]
    [DefaultValue("en-US")]
    public string LanguageCode { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the title displayed for the site.
    /// </summary>
    [Required]
    [DefaultValue("ForgingBlazor Application")]
    [MaxLength(100)]
    public string Title { get; set; } = "ForgingBlazor Application";
}
