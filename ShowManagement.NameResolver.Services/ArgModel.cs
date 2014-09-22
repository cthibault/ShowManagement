using System;
using System.Collections.Generic;
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
            ArgModel model = new ArgModel();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-i":
                    case "-install":
                        model.Install = true;
                        break;

                    case "-u":
                    case "-uninstall":
                        model.Uninstall = true;
                        break;

                    case "-c":
                    case "-console":
                        model.Console = true;
                        break;

                    case "-n":
                    case "-name":
                        if (args.Length > i + 1)
                        {
                            model.ServiceName = args[++i];
                        }
                        break;

                    default:
                        model._unexpectedArguments.Add(args[i]);
                        break;
                }
            }

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
