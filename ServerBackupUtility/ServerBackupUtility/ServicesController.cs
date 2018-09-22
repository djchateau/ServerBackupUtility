
using ServerBackupUtility.Logging;
using ServerBackupUtility.Services;
using System.Net;

namespace ServerBackupUtility
{
    public class ServicesController
    {
        private readonly ICompressionService _compressionService;
        private readonly ITransferService _transferService;
        private readonly IUploadService _uploadService;
        private readonly IDatabaseService _databaseService;
        private readonly IEmailService _emailService;

        public ServicesController()
        {
            _compressionService = new CompressionService();
            _transferService = new TransferService();
            _uploadService = new UploadService();
            _databaseService = new DatabaseService();
            _emailService = new EmailService();
        }

        public void RunBackup()
        {
            LogService.LogEvent();
            _compressionService.CreateArchives();

            LogService.LogEvent();

            if (_transferService.InitializeFtp())
            {
                LogService.LogEvent();
                _uploadService.UploadBackupFiles(_transferService);

                LogService.LogEvent();
                _databaseService.BackupDatabases(_transferService);
            }

            LogService.LogEvent("End Scheduled Global Server Backup");
            LogService.LogEvent();

            _emailService.CreateMessge();
        }
    }
}
