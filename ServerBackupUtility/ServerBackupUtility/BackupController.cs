
using System.Net;

namespace ServerBackupUtility
{
    public partial class BackupController
    {
        private readonly IArchiveService _archiveService;
        private readonly IFtpService _ftpService;
        private readonly IUploadService _uploadService;
        private readonly IDatabaseService _databaseService;
        private readonly ISmtpService _smtpService;

        public BackupController()
        {
            _archiveService = new ArchiveService();
            _ftpService = new FtpService();
            _uploadService = new UploadService();
            _databaseService = new DatabaseService();
            _smtpService = new SmtpService();
        }

        public void RunBackup()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            LogService.LogEventAsync().ConfigureAwait(true);
            _archiveService.CreateArchivesAsync().ConfigureAwait(true);
            LogService.LogEventAsync().ConfigureAwait(true);
            _ftpService.InitializeFtpAsync().ConfigureAwait(true);
            LogService.LogEventAsync().ConfigureAwait(true);
            _uploadService.UploadBackupFilesAsync(_ftpService).ConfigureAwait(true);
            LogService.LogEventAsync().ConfigureAwait(true);
            _databaseService.BackupDatabasesAsync(_ftpService).ConfigureAwait(true);
            LogService.LogEventAsync("End Scheduled Global Server Backup").ConfigureAwait(true);
            LogService.LogEventAsync().ConfigureAwait(true);
            _smtpService.CreateSmtpMessgeAsync().ConfigureAwait(true);
        }
    }
}
