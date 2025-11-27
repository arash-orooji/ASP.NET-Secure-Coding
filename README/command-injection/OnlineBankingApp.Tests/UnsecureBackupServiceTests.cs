using System;
using System.IO;
using System.Threading.Tasks;
using OnlineBankingApp.Services;
using Xunit;

namespace OnlineBankingApp.Tests;

public sealed class UnsecureBackupServiceTests
{
    [Fact]
    public async Task BackupDB_CommandInjectionPayload_InvokesCalcViaShell()
    {
        if (!OperatingSystem.IsWindows())
        {
            // This test validates Windows-specific shell behavior (cmd.exe).
            return;
        }

        var service = new UnsecureBackupService();
        var tempRoot = CreateTempDirectory();
        var originalDirectory = Directory.GetCurrentDirectory();

        // Prepare a stub for `calc` to avoid launching the real calculator and to capture invocation.
        var logPath = Path.Combine(tempRoot, "calc_invoked.log");
        var stubPath = Path.Combine(tempRoot, "calc.bat");
        await File.WriteAllTextAsync(stubPath, $"@echo off\r\necho CALC_CALLED > \"{logPath}\"\r\nexit /b 0\r\n");

        // Ensure our stub directory is first in PATH so it overrides any system-wide calc.exe.
        var originalPath = Environment.GetEnvironmentVariable("PATH");
        try
        {
            Environment.SetEnvironmentVariable("PATH", tempRoot + Path.PathSeparator + originalPath);

            // Set up working directory and seed database file
            Directory.SetCurrentDirectory(tempRoot);
            var sourcePath = Path.Combine(tempRoot, "OnlineBank.db");
            await File.WriteAllTextAsync(sourcePath, "seed-data");

            // Use a command injection payload that would trigger a second command (calc)
            await service.BackupDB("backup & calc");

            // If vulnerable, our stubbed calc.bat will be executed by the shell, creating the log file
            Assert.True(File.Exists(logPath));
            var contents = await File.ReadAllTextAsync(logPath);
            Assert.Contains("CALC_CALLED", contents);
        }
        finally
        {
            // Restore state
            Directory.SetCurrentDirectory(originalDirectory);
            Environment.SetEnvironmentVariable("PATH", originalPath);
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "OnlineBankingAppTests", Guid.NewGuid().ToString("N"));
        return Directory.CreateDirectory(tempPath).FullName;
    }
}
