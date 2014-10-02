using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.NameResolver.Service.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.NameResolver.Service.Program.Main()");
            bool allowRethrow = false;

            try
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Begin Parsing Arguments: {0}.", string.Join(" ", args));
                var model = ArgModel.Parse(args);
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Complete Parsing Arguments.");

                if (!string.IsNullOrWhiteSpace(model.ServiceName))
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Setting the Service Name = {0}.", model.ServiceName);
                    NameResolverWindowsService.SpecifiedServiceName = model.ServiceName;
                }

                if (model.Uninstall)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Uninstalling the Service.");
                    NameResolverWindowsService.Uninstall(args);

                    Program.PromptUserToClose(model.Auto);
                }

                if (model.Install)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Installing the Service.");
                    NameResolverWindowsService.Install(args);

                    Program.PromptUserToClose(model.Auto);
                }

                if (model.Console)
                {
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Hosting the service in the console window.");
                    Console.WriteLine("Starting Service in Console...");

                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the Service instance.");                    
                    var service = new NameResolverWindowsService();
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Starting the Service instance.");                    
                    service.TryStart();

                    Console.WriteLine("Service Running; press any key to stop.");
                    Console.ReadKey(true);

                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Stopping the Service instance.");
                    service.TryStop();

                    Console.WriteLine("Service Stopped");

                    Program.PromptUserToClose(model.Auto);
                }
                else if (!(model.Uninstall || model.Install))
                {
                    allowRethrow = true;

                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Initializing the Service instance.");
                    var ServicesToRun = new ServiceBase[] { new NameResolverWindowsService() };

                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Running the Service instance through the service base.");
                    ServiceBase.Run(ServicesToRun);

                    allowRethrow = false;
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.ExtractExceptionMessage();

                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Critical, 0, "Exception caught in ShowManagement.WindowsServices.NameResolver.Service.Program.Main(): {0}.", exceptionMessage);
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "allowRethrow value is {0}.", allowRethrow);

                if (allowRethrow)
                {
                    throw;
                }

                Console.Error.WriteLine(exceptionMessage);
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Exit ShowManagement.WindowsServices.NameResolver.Service.Program.Main()");
        }

        private static void PromptUserToClose(bool auto)
        {
            if (!auto)
            {
                Console.WriteLine("Press any key to exit console.");
                Console.ReadKey(true);
            }
        }
    }
}
