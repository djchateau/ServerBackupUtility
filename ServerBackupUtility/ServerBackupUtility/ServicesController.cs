
using ServerBackupUtility.Logging;
using ServerBackupUtility.Services;
using System.Net;

namespace ServerBackupUtility
{
    public class ServicesController
    {
        private readonly ICompressionService _compressionService;
        private readonly ITransferService _transferService;
        private readonly IDirectUploadService _directUploadService;
        private readonly IDatabaseUploadService _databaseUploadService;
        private readonly IEmailService _emailService;

        public ServicesController()
        {
            _compressionService = new CompressionService();
            _transferService = new TransferService();
            _directUploadService = new DirectUploadService();
            _databaseUploadService = new DatabaseUploadService();
            _emailService = new EmailService();
        }

        public void RunBackup()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            LogService.LogEvent();
            _compressionService.CreateArchives();
            LogService.LogEvent();
            _transferService.InitializeFtp();
            LogService.LogEvent();
            _directUploadService.UploadBackupFiles(_transferService);
            LogService.LogEvent();
            _databaseUploadService.BackupDatabases(_transferService);
            LogService.LogEvent("End Scheduled Global Server Backup");
            LogService.LogEvent();
            _emailService.CreateMessge();
        }
    }
}
