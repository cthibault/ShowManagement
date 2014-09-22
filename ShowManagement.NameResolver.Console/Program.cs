using ShowManagement.CommonServiceProviders;
using ShowManagement.NameResolver.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        private static async Task RunAsync()
        {
            var settings = new SettingsManager(
                new Dictionary<string, string>
                {
                    { SettingsManager.ITEM_RETRY_ATTEMPTS_KEY, "2" },
                    { SettingsManager.ITEM_RETRY_DURATION_KEY, "3" },
                });

            var nrService = new NameResolverEngine(settings, new ShowManagementServiceProvider());


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
                                nrService.Add(splitValues.Skip(1));
                                break;

                            case "R":
                                nrService.Remove(splitValues.Skip(1));
                                break;
                        }
                    }

                    break;
            }

            return quit;
        }
    }
}
