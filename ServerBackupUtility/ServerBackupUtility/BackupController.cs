
using System.Net;
using System.Threading.Tasks;

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

        public async Task RunBackupAsync()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            await LogService.CreateLogAsync("Begin Scheduled Global Server Backup").ConfigureAwait(false);
            await _archiveService.CreateArchivesAsync().ConfigureAwait(false);
            await LogService.LogEventAsync().ConfigureAwait(false);
            await _ftpService.InitializeFtpAsync().ConfigureAwait(false);
            await LogService.LogEventAsync().ConfigureAwait(false);
            await _uploadService.UploadBackupFilesAsync(_ftpService).ConfigureAwait(false);
            await LogService.LogEventAsync().ConfigureAwait(false);
            await _databaseService.BackupDatabasesAsync(_ftpService).ConfigureAwait(false);
            await LogService.LogEventAsync("End Scheduled Global Server Backup").ConfigureAwait(false);
            await LogService.LogEventAsync().ConfigureAwait(false);
            await _smtpService.CreateSmtpMessgeAsync().ConfigureAwait(false);
        }
    }
}
