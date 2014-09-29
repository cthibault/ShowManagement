using ShowManagement.CommonServiceProviders;
using ShowManagement.WindowsServices.NameResolver.Components;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.NameResolver.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            Program.SettingsManager = new SettingsManager(ConfigurationManager.AppSettings);

            var nrService = new NameResolverEngine(Program.SettingsManager, new ShowManagementServiceProvider(Program.SettingsManager.BaseAddress));

            var quit = false;

            while (!quit)
            {
                quit = ProcessInput(nrService);
            }            
        }

        private static bool ProcessInput(INameResolverEngine nrService)
        {
            bool quit = false;

            string input = System.Console.ReadLine();

            switch(input)
            {
                //QUIT
                case "Q":
                case "q":
                    quit = true;
                    break;

                //START
                case "START":
                case "start":
                    nrService.Start();
                    break;

                //STOP
                case "STOP":
                case "stop":
                    nrService.Stop();
                    break;

                default:
                    var splitValues = input.Split(',');

                    if (splitValues.Length > 0)
                    {
                        switch (splitValues[0].ToUpper())
                        {
                            case "A":
                                nrService.Add(splitValues.Skip(1).Select(v => Path.Combine(Program.SettingsManager.ParentDirectory, v)));
                                break;

                            case "R":
                                nrService.Remove(splitValues.Skip(1).Select(v => Path.Combine(Program.SettingsManager.ParentDirectory, v)));
                                break;
                        }
                    }

                    break;
            }

            return quit;
        }

        private static SettingsManager SettingsManager { get; set; }
    }
}
