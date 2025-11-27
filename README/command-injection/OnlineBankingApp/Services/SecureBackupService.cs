using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OnlineBankingApp.Services
{
    public class SecureBackupService
    {
        private static readonly Regex BackupNameRegex = new Regex(@"^[a-zA-Z0-9]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        public async Task BackupDB(string backupname)
        {
                if (!BackupNameRegex.IsMatch(backupname))
            {
                return;
            }

            var source = Path.Combine(Environment.CurrentDirectory, "OnlineBank.db");
            var backupsDirectory = Path.Combine(Environment.CurrentDirectory, "backups");
            Directory.CreateDirectory(backupsDirectory);

            var destination = Path.Combine(backupsDirectory, backupname);
            await FileCopyAsync(source, destination).ConfigureAwait(false);
        }

        public async Task FileCopyAsync(string sourceFileName, string destinationFileName, int bufferSize = 0x1000, CancellationToken cancellationToken = default)
        {
            await using var sourceFile = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            await using var destinationFile = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            await sourceFile.CopyToAsync(destinationFile, bufferSize, cancellationToken).ConfigureAwait(false);
        }        
        
    }

}