
using System.ServiceProcess;

namespace ServerBackupUtility
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[] 
			                { 
				                new Startup() 
			                };

            ServiceBase.Run(ServicesToRun);
        }
    }
}
