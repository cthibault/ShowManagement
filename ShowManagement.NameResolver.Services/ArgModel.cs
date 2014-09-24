using ShowManagement.NameResolver.Services.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services
{
    internal class ArgModel
    {
        private ArgModel() { }
        public static ArgModel Parse(string[] args)
        {
            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Enter ShowManagement.NameResolver.Services.ArgModel.Parse()");

            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Initializing the ArgModel instance.");
            ArgModel model = new ArgModel();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-i":
                    case "-install":
                        model.Install = true;
                        TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Install Flag set to TRUE.");
                        break;

                    case "-u":
                    case "-uninstall":
                        model.Uninstall = true;
                        TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Uninstall Flag set to TRUE.");
                        break;

                    case "-c":
                    case "-console":
                        model.Console = true;
                        TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Console Flag set to TRUE.");
                        break;

                    case "-n":
                    case "-name":
                        if (args.Length > i + 1)
                        {
                            model.ServiceName = args[++i];
                            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "ServiceName value set to {0}.", model.ServiceName);
                        }
                        else
                        {
                            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Warning, 0, "The 'Name Escape' parameter was found, but no value was provided.  Current index is {0}.  Parameter list: {1}", i, args);
                        }
                        break;

                    default:
                        model._unexpectedArguments.Add(args[i]);
                        TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Warning, 0, "Unexpected Parameter found: {0}.", args[i]);
                        break;
                }
            }

            TraceSourceManager.TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Exit ShowManagement.NameResolver.Services.ArgModel.Parse()");
            return model;
        }

        public bool Install { get; private set; }
        public bool Uninstall { get; private set; }
        public bool Console { get; private set; }
        public string ServiceName { get; private set; }

        public IEnumerable<string> UnexpectedArguments
        {
            get { return this._unexpectedArguments; }
        }
        private List<string> _unexpectedArguments = new List<string>();
    }
}
