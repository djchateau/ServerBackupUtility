
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

            await LogService.CreateLogAsync("Begin Scheduled Global Server Backup");
            await _archiveService.CreateArchivesAsync();
            await LogService.LogEventAsync();
            await _ftpService.InitializeFtpAsync();
            await LogService.LogEventAsync();
            await _uploadService.UploadBackupFilesAsync(_ftpService);
            await LogService.LogEventAsync();
            await _databaseService.BackupDatabasesAsync(_ftpService);
            await LogService.LogEventAsync("End Scheduled Global Server Backup");
            await LogService.LogEventAsync();
            await _smtpService.CreateSmtpMessgeAsync();
        }
    }
}
