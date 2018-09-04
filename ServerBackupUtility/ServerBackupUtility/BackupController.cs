
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

            LogService.CreateLogAsync("Scheduled Global Server Backup").ConfigureAwait(false);
            _archiveService.CreateArchivesAsync().ConfigureAwait(false);
            LogService.LogEventAsync().ConfigureAwait(false);
            _ftpService.InitializeFtpAsync().ConfigureAwait(false);
            LogService.LogEventAsync().ConfigureAwait(false);
            _uploadService.UploadBackupFilesAsync(_ftpService).ConfigureAwait(false);
            LogService.LogEventAsync().ConfigureAwait(false);
            _databaseService.BackupDatabasesAsync(_ftpService).ConfigureAwait(false);
            LogService.LogEventAsync("End Scheduled Global Server Backup").ConfigureAwait(false);
            LogService.LogEventAsync().ConfigureAwait(false);
            _smtpService.CreateSmtpMessgeAsync().ConfigureAwait(false);
        }
    }
}
