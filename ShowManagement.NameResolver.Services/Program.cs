using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
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
            bool allowRethrow = false;

            try
            {
                var model = ArgModel.Parse(args);

                if (!string.IsNullOrWhiteSpace(model.ServiceName))
                {
                    NameResolverWindowsService.CustomServiceName = model.ServiceName;
                }

                if (model.Uninstall)
                {
                    NameResolverWindowsService.Uninstall(args);
                    Console.WriteLine("Press any key to exit console.");
                    Console.ReadKey(true);
                }

                if (model.Install)
                {
                    NameResolverWindowsService.Install(args);
                    Console.WriteLine("Press any key to exit console.");
                    Console.ReadKey(true);
                }

                if (model.Console)
                {
                    Console.WriteLine("Starting Service in Console...");

                    var service = new NameResolverWindowsService();
                    service.Start();

                    Console.WriteLine("Service Running; press any key to stop.");
                    Console.ReadKey(true);

                    service.Stop();

                    Console.WriteLine("Service Stopped");
                }
                else if (!(model.Uninstall || model.Install))
                {
                    allowRethrow = true;

                    var ServicesToRun = new ServiceBase[] { new NameResolverWindowsService() };

                    ServiceBase.Run(ServicesToRun);

                    allowRethrow = false;
                }
            }
            catch (Exception ex)
            {
                if (allowRethrow)
                {
                    throw;
                }

                string exMessage = ex.ExtractExceptionMessage();

                Console.Error.WriteLine(exMessage);
            }
        }
    }
}
