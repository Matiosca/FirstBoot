namespace FirstBoot.Models;

/// <summary>
/// The applications available to install, grouped by category in the UI.
/// </summary>
public static class ApplicationCatalog
{
    public static IReadOnlyList<ApplicationModel> All { get; } =
    [
        new()
        {
            Name = "Steam",
            Category = "Gaming",
            WingetId = "Valve.Steam",
            Description = "PC game store and launcher.",
            Publisher = "Valve",
            Website = "https://store.steampowered.com/"
        },
        new()
        {
            Name = "Discord",
            Category = "Gaming",
            WingetId = "Discord.Discord",
            Description = "Voice, video, and text chat for communities.",
            Publisher = "Discord",
            Website = "https://discord.com/"
        },
        new()
    {
        Name = "TeamSpeak 3 Client",
        Category = "Gaming",
        WingetId = "TeamSpeakSystems.TeamSpeakClient",
        Description = "Low-latency voice chat for gaming communities.",
        Publisher = "TeamSpeak Systems GmbH",
        Website = "https://www.teamspeak.com/"
    },
        new()
        {
            Name = "7-Zip",
            Category = "Utilities",
            WingetId = "7zip.7zip",
            Description = "File archiver for creating and extracting compressed files.",
            Publisher = "Igor Pavlov",
            Website = "https://www.7-zip.org/"
        },
        new()
        {
            Name = "PowerToys",
            Category = "Utilities",
            WingetId = "Microsoft.PowerToys",
            Description = "A collection of productivity utilities for Windows.",
            Publisher = "Microsoft",
            Website = "https://learn.microsoft.com/windows/powertoys/"
        },
        new()
        {
            Name = "Visual Studio Code",
            Category = "Development",
            WingetId = "Microsoft.VisualStudioCode",
            Description = "Lightweight code editor with extension support.",
            Publisher = "Microsoft",
            Website = "https://code.visualstudio.com/"
        },
        new()
        {
            Name = "Git",
            Category = "Development",
            WingetId = "Git.Git",
            Description = "Distributed version control system.",
            Publisher = "Git for Windows",
            Website = "https://git-scm.com/"
        },
        new()
        {
            Name = "Google Chrome",
            Category = "Browsers",
            WingetId = "Google.Chrome",
            Description = "Google's web browser.",
            Publisher = "Google",
            Website = "https://www.google.com/chrome/"
        },
        new()
        {
            Name = "Mozilla Firefox",
            Category = "Browsers",
            WingetId = "Mozilla.Firefox",
            Description = "Privacy-focused web browser.",
            Publisher = "Mozilla",
            Website = "https://www.mozilla.org/firefox/"
        },
        new()
        {
            Name = "VLC media player",
            Category = "Media",
            WingetId = "VideoLAN.VLC",
            Description = "Open-source media player for audio and video.",
            Publisher = "VideoLAN",
            Website = "https://www.videolan.org/vlc/"
        },
        new()
        {
            Name = "Spotify",
            Category = "Media",
            WingetId = "Spotify.Spotify",
            Description = "Music and podcast streaming application.",
            Publisher = "Spotify",
            Website = "https://www.spotify.com/"
        },
        new()
        {
            Name = "NVIDIA App",
            Category = "Hardware / System",
            WingetId = "Nvidia.NvidiaApp",
            Description = "NVIDIA graphics driver and GPU settings companion.",
            Publisher = "NVIDIA",
            Website = "https://www.nvidia.com/en-us/software/nvidia-app/"
        },
        new()
        {
            Name = "AMD Software: Adrenalin Edition",
            Category = "Hardware / System",
            WingetId = "AMD.AMDSoftware",
            Description = "AMD graphics driver and settings software.",
            Publisher = "AMD",
            Website = "https://www.amd.com/en/support"
        }
    ];
}
