
using System.ComponentModel;

namespace ServerBackupUtility
{
    partial class Startup
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            this.ServiceName = "BackupScheduler";
        }
    }
}
