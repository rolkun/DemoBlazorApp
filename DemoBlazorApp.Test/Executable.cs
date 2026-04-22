using System.Diagnostics;

internal class Executable
{
    private readonly string fullPath;
    private readonly string workingDirectory;
    private Process? process;

    public Executable(string fullPath)
    {
        this.fullPath = fullPath;
        this.workingDirectory = Path.GetDirectoryName(fullPath)!;
    }

    public async Task StartAsync(string arguments = "")
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
