/// <summary>
/// The applications available to install, grouped by category in the UI.
/// </summary>

using FirstBoot.Models;

public static class ApplicationCatalog
{
    public static IReadOnlyList<ApplicationModel> All { get; } =
    [
        // ==================================================
        // GAMING
        // ==================================================

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
            Name = "Epic Games Launcher",
            Category = "Gaming",
            WingetId = "EpicGames.EpicGamesLauncher",
            Description = "Game launcher and digital game store from Epic Games.",
            Publisher = "Epic Games",
            Website = "https://store.epicgames.com/"
        },

        new()
        {
            Name = "EA app",
            Category = "Gaming",
            WingetId = "ElectronicArts.EADesktop",
            Description = "EA's PC game launcher and digital distribution platform.",
            Publisher = "Electronic Arts",
            Website = "https://www.ea.com/ea-app"
        },

        new()
        {
            Name = "Battle.net",
            Category = "Gaming",
            WingetId = "Blizzard.BattleNet",
            Description = "Blizzard Entertainment's game launcher and digital distribution platform.",
            Publisher = "Blizzard Entertainment",
            Website = "https://download.battle.net/"
        },

        new()
        {
            Name = "Rockstar Games Launcher",
            Category = "Gaming",
            WingetId = "RockstarGames.Launcher",
            Description = "Rockstar Games' launcher for accessing and managing PC games.",
            Publisher = "Rockstar Games",
            Website = "https://launcher.rockstargames.com/"
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
            Description = "Low-latency voice communication for gaming communities.",
            Publisher = "TeamSpeak Systems GmbH",
            Website = "https://www.teamspeak.com/"
        },


        // ==================================================
        // UTILITIES
        // ==================================================

        new()
        {
            Name = "qBittorrent",
            Category = "Utilities",
            WingetId = "qBittorrent.qBittorrent",
            Description = "Open-source BitTorrent client.",
            Publisher = "The qBittorrent Project",
            Website = "https://www.qbittorrent.org/"
        },

        new()
        {
            Name = "Proton VPN",
            Category = "Utilities",
            WingetId = "Proton.ProtonVPN",
            Description = "VPN application focused on privacy and secure internet access.",
            Publisher = "Proton",
            Website = "https://protonvpn.com/"
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
            Name = "CPU-Z",
            Category = "Utilities",
            WingetId = "CPUID.CPU-Z",
            Description = "System information utility providing detailed CPU, motherboard, memory, and system information.",
            Publisher = "CPUID",
            Website = "https://www.cpuid.com/softwares/cpu-z.html"
        },

        new()
        {
            Name = "Calibre",
            Category = "Utilities",
            WingetId = "calibre.calibre",
            Description = "E-book management, conversion, and reading software.",
            Publisher = "Kovid Goyal",
            Website = "https://calibre-ebook.com/"
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


        // ==================================================
        // DEVELOPMENT
        // ==================================================

        new()
        {
            Name = "Arduino IDE",
            Category = "Development",
            WingetId = "ArduinoSA.IDE.stable",
            Description = "Integrated development environment for Arduino boards and compatible microcontrollers.",
            Publisher = "Arduino SA",
            Website = "https://www.arduino.cc/en/software"
        },

        new()
        {
            Name = "Python 3.13",
            Category = "Development",
            WingetId = "Python.Python.3.13",
            Description = "Python programming language and development environment.",
            Publisher = "Python Software Foundation",
            Website = "https://www.python.org/"
        },

        new()
        {
            Name = "Visual Studio Code",
            Category = "Development",
            WingetId = "Microsoft.VisualStudioCode",
            Description = "Lightweight source-code editor with extensive development and extension support.",
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


        // ==================================================
        // BROWSERS
        // ==================================================

        new()
        {
            Name = "Mozilla Firefox",
            Category = "Browsers",
            WingetId = "Mozilla.Firefox",
            Description = "Open-source web browser developed by Mozilla.",
            Publisher = "Mozilla",
            Website = "https://www.mozilla.org/firefox/"
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


        // ==================================================
        // MEDIA
        // ==================================================

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
            Name = "VLC media player",
            Category = "Media",
            WingetId = "VideoLAN.VLC",
            Description = "Open-source multimedia player supporting a wide range of audio and video formats.",
            Publisher = "VideoLAN",
            Website = "https://www.videolan.org/vlc/"
        },


        // ==================================================
        // HARDWARE / SYSTEM
        // ==================================================

        new()
        {
            Name = "NVIDIA App",
            Category = "Hardware / System",
            WingetId = "Nvidia.NvidiaApp",
            Description = "NVIDIA graphics driver, settings, and companion application.",
            Publisher = "NVIDIA",
            Website = "https://www.nvidia.com/en-us/software/nvidia-app/"
        },

        new()
        {
            Name = "TegraRcmGUI",
            Category = "Hardware / System",
            WingetId = "eliboa.TegraRcmGUI",
            Description = "Graphical interface for sending payloads to compatible NVIDIA Tegra devices.",
            Publisher = "eliboa",
            Website = "https://github.com/eliboa/TegraRcmGUI"
        }
    ];
}