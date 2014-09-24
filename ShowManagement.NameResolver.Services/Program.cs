﻿using ShowManagement.Core.Extensions;
using ShowManagement.NameResolver.Services.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services
{
    class Program
    {
        static void Main(string[] args)
        {
            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter ShowManagement.NameResolver.Services.Program.Main()");
            bool allowRethrow = false;

            try
            {
                TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Begin Parsing Arguments: {0}.", args);
                var model = ArgModel.Parse(args);
                TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Complete Parsing Arguments.");

                if (!string.IsNullOrWhiteSpace(model.ServiceName))
                {
                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Information, 0, "Setting the Service Name = {0}.", model.ServiceName);
                    NameResolverWindowsService.SpecifiedServiceName = model.ServiceName;
                }

                if (model.Uninstall)
                {
                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Information, 0, "Uninstalling the Service.");
                    NameResolverWindowsService.Uninstall(args);
                    Console.WriteLine("Press any key to exit console.");
                    Console.ReadKey(true);
                }

                if (model.Install)
                {
                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Information, 0, "Installing the Service.");
                    NameResolverWindowsService.Install(args);
                    Console.WriteLine("Press any key to exit console.");
                    Console.ReadKey(true);
                }

                if (model.Console)
                {
                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Information, 0, "Hosting the service in the console window.");
                    Console.WriteLine("Starting Service in Console...");

                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Initializing the Service instance.");                    
                    var service = new NameResolverWindowsService();
                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Starting the Service instance.");                    
                    service.Start();

                    Console.WriteLine("Service Running; press any key to stop.");
                    Console.ReadKey(true);

                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Stopping the Service instance.");
                    service.Stop();

                    Console.WriteLine("Service Stopped");
                }
                else if (!(model.Uninstall || model.Install))
                {
                    allowRethrow = true;

                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Initializing the Service instance.");
                    var ServicesToRun = new ServiceBase[] { new NameResolverWindowsService() };

                    TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Running the Service instance through the service base.");
                    ServiceBase.Run(ServicesToRun);

                    allowRethrow = false;
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.ExtractExceptionMessage();

                TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Critical, 0, "Exception caught in ShowManagement.NameResolver.Services.Program.Main(): {0}.", exceptionMessage);
                TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "allowRethrow value is {0}.", allowRethrow);

                if (allowRethrow)
                {
                    throw;
                }

                Console.Error.WriteLine(exceptionMessage);
            }

            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Exit ShowManagement.NameResolver.Services.Program.Main()");
        }
    }
}
