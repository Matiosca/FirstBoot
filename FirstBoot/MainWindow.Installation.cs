using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FirstBoot.Models;

namespace FirstBoot;

public partial class MainWindow
{
    private const string AppInstallerBundleUrl =
        "https://github.com/microsoft/winget-cli/releases/latest/download/Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.msixbundle";

    private const string AppInstallerDependenciesUrl =
        "https://github.com/microsoft/winget-cli/releases/latest/download/DesktopAppInstaller_Dependencies.zip";

    private static readonly HttpClient DownloadClient = new();

    private CancellationTokenSource? _installationCancellation;

    private async void InstallSelectedButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedApplications = ApplicationCatalog.All
            .Where(app => app.IsSelected)
            .ToList();

        if (selectedApplications.Count == 0)
        {
            MessageBox.Show(
                "Select at least one application before installing.",
                "No applications selected",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        if (!await IsWingetAvailableAsync())
        {
            var installWinget = MessageBox.Show(
                "Windows Package Manager (winget) is not available. Would you like to download and install Microsoft's App Installer package now?",
                "Winget is unavailable",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (installWinget != MessageBoxResult.Yes)
            {
                return;
            }

            InstallSelectedButton.IsEnabled = false;
            InstallationStatusText.Text = "Downloading Windows Package Manager...";

            var bootstrapSucceeded = await BootstrapWingetAsync();

            InstallationStatusText.Text = string.Empty;
            UpdateSelectedApplicationSummary();

            if (!bootstrapSucceeded || !await IsWingetAvailableAsync())
            {
                MessageBox.Show(
                    "Windows Package Manager could not be installed automatically. Install or update App Installer from the Microsoft Store, then try again.",
                    "Winget installation failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }
        }

        var confirmation = MessageBox.Show(
            $"Install {selectedApplications.Count} selected application{(selectedApplications.Count == 1 ? string.Empty : "s")}?",
            "Confirm installation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmation != MessageBoxResult.Yes)
        {
            return;
        }

        InstallSelectedButton.IsEnabled = false;
        CancelInstallationButton.IsEnabled = true;

        using var cancellation = new CancellationTokenSource();
        _installationCancellation = cancellation;

        var results = new List<InstallationResult>();
        var wasCancelled = false;

        for (var index = 0; index < selectedApplications.Count; index++)
        {
            var application = selectedApplications[index];

            InstallationStatusText.Text =
                $"Checking {index + 1} of {selectedApplications.Count}: {application.Name}";

            try
            {
                if (await IsApplicationInstalledAsync(application, cancellation.Token))
                {
                    results.Add(
                        new InstallationResult(
                            application.Name,
                            true,
                            "Application was already installed.",
                            true));

                    application.IsSelected = false;
                    continue;
                }

                InstallationStatusText.Text =
                    $"Installing {index + 1} of {selectedApplications.Count}: {application.Name}";

                var result = await InstallApplicationAsync(
                    application,
                    cancellation.Token);

                results.Add(result);

                if (result.Succeeded)
                {
                    application.IsSelected = false;
                }
            }
            catch (OperationCanceledException)
            {
                wasCancelled = true;
                break;
            }
            catch (Exception exception)
            {
                results.Add(
                    new InstallationResult(
                        application.Name,
                        false,
                        exception.ToString(),
                        false));

                Debug.WriteLine(
                    $"Unable to install {application.Name}: {exception}");
            }
        }

        var logPath = await WriteInstallationLogAsync(results);

        InstallationStatusText.Text = string.Empty;
        InstallSelectedButton.IsEnabled = true;
        CancelInstallationButton.IsEnabled = false;
        _installationCancellation = null;

        UpdateSelectedApplicationSummary();

        var failedResults = results
    .Where(result => !result.Succeeded)
    .ToList();

        var installedResults = results
            .Where(result => result.Succeeded && !result.AlreadyInstalled)
            .ToList();

        var alreadyInstalledResults = results
            .Where(result => result.AlreadyInstalled)
            .ToList();

        ShowInstallationSummary(results, wasCancelled);

        var message = wasCancelled
            ? "Installation was cancelled. Applications that were not installed remain selected, so you can try again."
            : failedResults.Count > 0
                ? $"Installation finished with errors.\n\n" +
                  $"Installed: {installedResults.Count}\n" +
                  $"Already installed: {alreadyInstalledResults.Count}\n" +
                  $"Failed: {failedResults.Count}\n\n" +
                  $"Failed applications:\n{string.Join("\n", failedResults.Select(result => result.ApplicationName))}\n\n" +
                  "Failed applications remain selected, so you can try again."
                : installedResults.Count > 0 && alreadyInstalledResults.Count > 0
                    ? $"Installation complete.\n\n" +
                      $"Installed: {installedResults.Count}\n" +
                      $"Already installed: {alreadyInstalledResults.Count}"
                    : alreadyInstalledResults.Count > 0
                        ? "All selected applications were already installed."
                        : "All selected applications were installed successfully.";

        if (logPath is not null)
        {
            message += $"\n\nInstallation log:\n{logPath}";
        }

        MessageBox.Show(
            message,
            wasCancelled
                ? "Installation cancelled"
                : failedResults.Count == 0
                    ? "Installation complete"
                    : "Installation complete with errors",
            MessageBoxButton.OK,
            wasCancelled || failedResults.Count > 0
                ? MessageBoxImage.Warning
                : MessageBoxImage.Information);
    }

    private void CancelInstallationButton_Click(object sender, RoutedEventArgs e)
    {
        _installationCancellation?.Cancel();

        CancelInstallationButton.IsEnabled = false;
        InstallationStatusText.Text = "Cancelling installation...";
    }

    private void ShowInstallationSummary(
        IEnumerable<InstallationResult> results,
        bool wasCancelled)
    {
        InformationPanel.Children.Clear();

        InformationPanel.Children.Add(
            new TextBlock
            {
                Text = wasCancelled
                    ? "INSTALLATION CANCELLED"
                    : "INSTALLATION RESULTS",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            });

        foreach (var result in results)
        {
            var status = result.AlreadyInstalled
                ? "ALREADY INSTALLED"
                : result.Succeeded
                    ? "INSTALLED"
                    : "FAILED";

            InformationPanel.Children.Add(
                new TextBlock
                {
                    Text = $"{(result.Succeeded ? "✓" : "✗")} {result.ApplicationName} — {status}",
                    Foreground = result.Succeeded
                        ? Brushes.ForestGreen
                        : Brushes.Firebrick,
                    FontWeight = FontWeights.SemiBold,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 6)
                });
        }
    }

    private static async Task<bool> IsWingetAvailableAsync()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("--version");

            using var process = Process.Start(startInfo);

            if (process is null)
            {
                return false;
            }

            await process.WaitForExitAsync();

            return process.ExitCode == 0;
        }
        catch (Exception exception)
            when (exception is InvalidOperationException
                or System.ComponentModel.Win32Exception)
        {
            return false;
        }
    }

    private static async Task<bool> IsApplicationInstalledAsync(
        ApplicationModel application,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("list");
        startInfo.ArgumentList.Add("--id");
        startInfo.ArgumentList.Add(application.WingetId);
        startInfo.ArgumentList.Add("--exact");
        startInfo.ArgumentList.Add("--accept-source-agreements");

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "Windows Package Manager (winget) could not be started.");

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        var output = await outputTask;
        var error = await errorTask;

        Debug.WriteLine(
            $"Winget list result for {application.Name}:{Environment.NewLine}" +
            $"{output}{Environment.NewLine}{error}");

        return process.ExitCode == 0 &&
               output.Contains(
                   application.WingetId,
                   StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<bool> BootstrapWingetAsync()
    {
        var workingDirectory = Path.Combine(
            Path.GetTempPath(),
            $"FirstBoot-{Guid.NewGuid():N}");

        try
        {
            Directory.CreateDirectory(workingDirectory);

            var bundlePath = Path.Combine(
                workingDirectory,
                "Microsoft.DesktopAppInstaller.msixbundle");

            var dependenciesArchivePath = Path.Combine(
                workingDirectory,
                "Dependencies.zip");

            var dependenciesDirectory = Path.Combine(
                workingDirectory,
                "Dependencies");

            await DownloadFileAsync(
                AppInstallerBundleUrl,
                bundlePath);

            await DownloadFileAsync(
                AppInstallerDependenciesUrl,
                dependenciesArchivePath);

            ZipFile.ExtractToDirectory(
                dependenciesArchivePath,
                dependenciesDirectory);

            var dependencyPaths = Directory
                .EnumerateFiles(
                    dependenciesDirectory,
                    "*.*",
                    SearchOption.AllDirectories)
                .Where(path =>
                    path.EndsWith(".appx", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".msix", StringComparison.OrdinalIgnoreCase) ||
                    path.EndsWith(".msixbundle", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (dependencyPaths.Count == 0)
            {
                return false;
            }

            var escapedBundlePath =
                EscapePowerShellString(bundlePath);

            var escapedDependencyPaths =
                string.Join(
                    ", ",
                    dependencyPaths.Select(
                        path => $"'{EscapePowerShellString(path)}'"));

            var command =
                $"Add-AppxPackage -Path '{escapedBundlePath}' " +
                $"-DependencyPath @({escapedDependencyPaths})";

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            startInfo.ArgumentList.Add("-Command");
            startInfo.ArgumentList.Add(command);

            using var process = Process.Start(startInfo);

            if (process is null)
            {
                return false;
            }

            var outputTask =
                process.StandardOutput.ReadToEndAsync();

            var errorTask =
                process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            Debug.WriteLine(
                $"Winget bootstrap output: " +
                $"{await outputTask}\n{await errorTask}");

            return process.ExitCode == 0;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(
                $"Unable to bootstrap winget: {exception}");

            return false;
        }
        finally
        {
            try
            {
                if (Directory.Exists(workingDirectory))
                {
                    Directory.Delete(
                        workingDirectory,
                        recursive: true);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(
                    $"Unable to remove Winget bootstrap files: {exception}");
            }
        }
    }

    private static async Task DownloadFileAsync(
        string sourceUrl,
        string destinationPath)
    {
        using var response = await DownloadClient.GetAsync(
            sourceUrl,
            HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        await using var source =
            await response.Content.ReadAsStreamAsync();

        await using var destination =
            File.Create(destinationPath);

        await source.CopyToAsync(destination);
    }

    private static string EscapePowerShellString(string value)
    {
        return value.Replace("'", "''");
    }

    private static async Task<InstallationResult> InstallApplicationAsync(
        ApplicationModel application,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("install");
        startInfo.ArgumentList.Add("--id");
        startInfo.ArgumentList.Add(application.WingetId);
        startInfo.ArgumentList.Add("--exact");
        startInfo.ArgumentList.Add("--silent");
        startInfo.ArgumentList.Add("--accept-package-agreements");
        startInfo.ArgumentList.Add("--accept-source-agreements");

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "Windows Package Manager (winget) could not be started.");

        var standardOutputTask =
            process.StandardOutput.ReadToEndAsync();

        var standardErrorTask =
            process.StandardError.ReadToEndAsync();

        try
        {
            await process.WaitForExitAsync(
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                process.Kill(
                    entireProcessTree: true);
            }

            await process.WaitForExitAsync();

            throw;
        }

        var output =
            $"{await standardOutputTask}\n" +
            $"{await standardErrorTask}".Trim();

        return new InstallationResult(
            application.Name,
            process.ExitCode == 0,
            output,
            false);
    }

    private static async Task<string?> WriteInstallationLogAsync(
        IEnumerable<InstallationResult> results)
    {
        try
        {
            var logDirectory = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "FirstBoot");

            Directory.CreateDirectory(logDirectory);

            var logPath = Path.Combine(
                logDirectory,
                "installation.log");

            var log = new StringBuilder();

            log.AppendLine(
                $"{DateTimeOffset.Now:O} Installation run");

            foreach (var result in results)
            {
                var status = result.AlreadyInstalled
                    ? "ALREADY INSTALLED"
                    : result.Succeeded
                        ? "SUCCESS"
                        : "FAILED";

                log.AppendLine(
                    $"[{status}] {result.ApplicationName}");

                log.AppendLine(result.Output);
                log.AppendLine();
            }

            await File.AppendAllTextAsync(
                logPath,
                log.ToString());

            return logPath;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(
                $"Unable to write installation log: {exception}");

            return null;
        }
    }

    private sealed record InstallationResult(
        string ApplicationName,
        bool Succeeded,
        string Output,
        bool AlreadyInstalled);
}