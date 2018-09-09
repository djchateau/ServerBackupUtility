
using ServerBackupUtility.Logging;
using ServerBackupUtility.Services;
using System.Net;

namespace ServerBackupUtility
{
    public class BackupController
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

            LogService.LogEvent();
            _archiveService.CreateArchives();
            LogService.LogEvent();
            _ftpService.InitializeFtp();
            LogService.LogEvent();
            _uploadService.UploadBackupFiles(_ftpService);
            LogService.LogEvent();
            _databaseService.BackupDatabases(_ftpService);
            LogService.LogEvent("End Scheduled Global Server Backup");
            LogService.LogEvent();
            _smtpService.CreateSmtpMessge();
        }
    }
}
