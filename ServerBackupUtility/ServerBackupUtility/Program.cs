
using System;
using System.Threading.Tasks;

namespace ServerBackupUtility
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LogService.LogEventAsync("Error: Program.Main - " + ex.Message).ConfigureAwait(false);
                Environment.Exit(127);
            }

            Environment.Exit(0);

        }

        private static Task MainAsync(string[] args)
        {
            IServerBackupService serverBackupUtility = new ServerBackupService();
            return serverBackupUtility.RunServerBackupAsync();
        }
    }
}
