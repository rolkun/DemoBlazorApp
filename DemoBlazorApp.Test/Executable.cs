using System.Diagnostics;

internal class Executable
{
    private readonly string fullPath;
    private readonly string workingDirectory;
    private Process? process;

    // fullPath: executable to run (e.g. path to exe or "dotnet")
    // workingDirectory: directory where the process will be started (if null, derived from fullPath)
    public Executable(string fullPath, string? workingDirectory = null)
    {
        this.fullPath = fullPath;
        if (!string.IsNullOrEmpty(workingDirectory))
            this.workingDirectory = workingDirectory;
        else
            this.workingDirectory = Path.GetDirectoryName(fullPath)!;
    }

    public async Task StartAsync(string arguments = "", IDictionary<string, string>? environment = null)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fullPath,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (environment != null)
        {
            foreach (var (key, value) in environment)
                psi.Environment[key] = value;
        }

        process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        process.Start();

        // Async lesen, damit nichts blockiert
        _ = Task.Run(() => process.StandardOutput.ReadToEndAsync());
        _ = Task.Run(() => process.StandardError.ReadToEndAsync());

        await Task.Delay(200); // kurze Stabilisierung
    }

    public async Task StopAsync()
    {
        if (process == null || process.HasExited)
            return;

        try
        {
            process.CloseMainWindow(); // funktioniert bei GUI-Apps
        }
        catch { }

        // sanft beenden
        if (!process.HasExited)
        {
            process.Kill(true);
        }

        await Task.Delay(200);
    }
}
