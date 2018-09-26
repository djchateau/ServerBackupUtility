
using ServerBackupUtility.Logging;
using ServerBackupUtility.Services;
using System;
using System.Net;

namespace ServerBackupUtility
{
    public class ServicesController
    {
        private readonly IArchiveService _archiveService;
        private readonly ITransferService _transferService;
        private readonly IUploadService _uploadService;
        private readonly IDatabaseService _databaseService;
        private readonly IEmailService _emailService;

        public ServicesController()
        {
            _archiveService = new ArchiveService();
            _transferService = new TransferService();
            _uploadService = new UploadService();
            _databaseService = new DatabaseService();
            _emailService = new EmailService();
        }

        public void RunBackup()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            try
            {
                LogService.LogEvent();
                _archiveService.CreateArchives();

                LogService.LogEvent();

                if (_transferService.InitializeFtpAsync().Result)
                {
                    _uploadService.UploadBackupFiles(_transferService);

                    LogService.LogEvent();
                    _databaseService.BackupDatabases(_transferService);
                }

                LogService.LogEvent("End Scheduled Global Server Backup");
                LogService.LogEvent();

                _emailService.CreateMessge();
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: ServicesController.RunBackup - " + ex.Message);
            }
        }
    }
}
