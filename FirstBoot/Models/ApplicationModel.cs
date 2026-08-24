namespace FirstBoot.Models;

/// <summary>
/// Describes an application that can be selected and installed.
/// </summary>
public sealed class ApplicationModel
{
    /// <summary>The name shown in the installer.</summary>
    public required string Name { get; init; }

    /// <summary>The category used to group the application.</summary>
    public required string Category { get; init; }

    /// <summary>The package identifier used by Winget.</summary>
    public required string WingetId { get; init; }

    /// <summary>A short explanation of what the application is for.</summary>
    public required string Description { get; init; }

    /// <summary>An optional publisher or vendor label.</summary>
    public string? Publisher { get; init; }

    /// <summary>An optional URL for the application's website.</summary>
    public string? Website { get; init; }

    /// <summary>Whether this application has been selected for installation.</summary>
    public bool IsSelected { get; set; }
}
