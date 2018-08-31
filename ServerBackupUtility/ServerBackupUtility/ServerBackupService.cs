
using System;
using System.Net;
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    public class ServerBackupService : IServerBackupService
    {
        private readonly IArchiveService _archiveService;
        private readonly IFtpService _ftpService;
        private readonly IUploadService _uploadService;
        private readonly IDatabaseService _databaseService;
        private readonly ISmtpService _smtpService;

        public ServerBackupService()
        {
            _archiveService = new ArchiveService();
            _ftpService = new FtpService();
            _uploadService = new UploadService();
            _databaseService = new DatabaseService();
            _smtpService = new SmtpService();
        }

        public async Task RunServerBackupAsync()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            await LogService.CreateLogAsync("Begin Scheduled Global Server Backup").ConfigureAwait(true);
            await _archiveService.CreateArchivesAsync().ConfigureAwait(true);
            await LogService.LogEventAsync().ConfigureAwait(true);
            await _ftpService.InitializeFtpAsync().ConfigureAwait(true);
            await LogService.LogEventAsync().ConfigureAwait(true);
            await _uploadService.UploadBackupFilesAsync(_ftpService).ConfigureAwait(true);
            await LogService.LogEventAsync().ConfigureAwait(true);
            await _databaseService.BackupDatabasesAsync(_ftpService).ConfigureAwait(true);
            await LogService.LogEventAsync("End Scheduled Global Server Backup").ConfigureAwait(true);
            await LogService.LogEventAsync().ConfigureAwait(true);
            await _smtpService.CreateSmtpMessgeAsync().ConfigureAwait(true);
        }
    }
}
