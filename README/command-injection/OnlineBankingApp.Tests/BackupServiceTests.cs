using System;
using System.IO;
using System.Threading.Tasks;
using OnlineBankingApp.Services;
using Xunit;

namespace OnlineBankingApp.Tests;

public sealed class BackupServiceTests
{
    [Fact]
    public async Task BackupDB_WithValidName_CreatesBackupFile()
    {
        var service = new SecureBackupService();
        var tempRoot = CreateTempDirectory();
        var originalDirectory = Directory.GetCurrentDirectory();

        try
        {
            Directory.SetCurrentDirectory(tempRoot);
            var sourcePath = Path.Combine(tempRoot, "OnlineBank.db");
            await File.WriteAllTextAsync(sourcePath, "seed-data");

            await service.BackupDB("Backup001");

            var expectedBackup = Path.Combine(tempRoot, "backups", "Backup001");
            Assert.True(File.Exists(expectedBackup));
            var copiedContent = await File.ReadAllTextAsync(expectedBackup);
            Assert.Equal("seed-data", copiedContent);
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    [Fact]
    public async Task BackupDB_WithInvalidName_DoesNotCreateBackup()
    {
        var service = new SecureBackupService();
        var tempRoot = CreateTempDirectory();
        var originalDirectory = Directory.GetCurrentDirectory();

        try
        {
            Directory.SetCurrentDirectory(tempRoot);
            await service.BackupDB("invalid-name!");
            var backupDirectory = Path.Combine(tempRoot, "backups");
            Assert.False(Directory.Exists(backupDirectory));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    private static string CreateTempDirectory()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "OnlineBankingAppTests", Guid.NewGuid().ToString("N"));
        return Directory.CreateDirectory(tempPath).FullName;
    }
}
