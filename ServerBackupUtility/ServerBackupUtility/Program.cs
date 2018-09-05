
using System;
using System.Reflection;
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

            if (Environment.UserInteractive && System.Diagnostics.Debugger.IsAttached)
            {
                // Simulate the services execution
                RunInteractiveServices(ServicesToRun);
            }
            else
            {
                // Normal service execution
                ServiceBase.Run(ServicesToRun);
            }
        }

        static void RunInteractiveServices(ServiceBase[] servicesToRun)
        {
            Console.WriteLine();
            Console.WriteLine("Starting the Service in Interactive Mode");
            Console.WriteLine();

            // Get the method to invoke on each service to start it
            MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);

            // Start services loop
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write(service.ServiceName);
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Console.WriteLine(" Started");
            }

            // Waiting for the end
            Console.WriteLine();
            Console.WriteLine("Press a Key to Stop the Service and Finish the Process");
            Console.ReadKey();
            Console.WriteLine();

            // Get the method to invoke on each service to stop it.
            MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);

            // Stop loop
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write(service.ServiceName);
                onStopMethod.Invoke(service, null);
                Console.WriteLine(" Stopped");
            }

            Console.WriteLine();
            Console.WriteLine("Server Backup Utility Has Completed");

            // Waiting a key press to not return to VS directly
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.Write("Press any Key to Exit");
                Console.ReadKey();
            }
        }
    }
}
