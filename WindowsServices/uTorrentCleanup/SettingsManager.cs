using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.WindowsServices.uTorrentCleanup
{
    public class SettingsManager
    {
        public SettingsManager(IDictionary<string, string> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this._settingsDictionary = settings;
        }
        public SettingsManager(NameValueCollection settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this._settingsDictionary = new Dictionary<string, string>(settings.Count);

            foreach (var key in settings.AllKeys)
            {
                this._settingsDictionary.Add(key, settings[key]);
            }
        }

        #region Public Properties

        public string WebUiBaseAddress
        {
            get
            {
                if (this._webuiBaseAddress == null)
                {
                    this._webuiBaseAddress = string.Empty;
                    if (this._settingsDictionary.ContainsKey(WEBUI_BASE_ADDRESS))
                    {
                        _webuiBaseAddress = this._settingsDictionary[WEBUI_BASE_ADDRESS];
                    }
                }

                return this._webuiBaseAddress;
            }
        }

        public int WebUiPort
        {
            get
            {
                if (!this._webuiPort.HasValue)
                {
                    string settingValue = string.Empty;
                    if (this._settingsDictionary.ContainsKey(WEBUI_PORT))
                    {
                        settingValue = this._settingsDictionary[WEBUI_PORT];
                    }

                    int convertedValue = 0;
                    this._webuiPort = int.TryParse(settingValue, out convertedValue) ? convertedValue : WEBUI_PORT_DEFAULT_VALUE;
                }

                return this._webuiPort.Value;
            }
        }

        public string WebUiUser
        {
            get
            {
                if (this._webuiUser == null)
                {
                    this._webuiUser = string.Empty;
                    if (this._settingsDictionary.ContainsKey(WEBUI_USER))
                    {
                        _webuiUser = this._settingsDictionary[WEBUI_USER];
                    }
                }

                return this._webuiUser;
            }
        }

        public string WebUiPassword
        {
            get
            {
                if (this._webuiPassword == null)
                {
                    this._webuiPassword = string.Empty;
                    if (this._settingsDictionary.ContainsKey(WEBUI_PASSWORD))
                    {
                        _webuiPassword = this._settingsDictionary[WEBUI_PASSWORD];
                    }
                }

                return this._webuiPassword;
            }
        }

        public int CleanupIntervalInSeconds
        {
            get
            {
                if (!this._cleanupIntervalInSeconds.HasValue)
                {
                    string settingValue = string.Empty;
                    if (this._settingsDictionary.ContainsKey(CLEANUP_INTERVAL))
                    {
                        settingValue = this._settingsDictionary[CLEANUP_INTERVAL];
                    }

                    int convertedValue = 0;
                    this._cleanupIntervalInSeconds = int.TryParse(settingValue, out convertedValue) ? convertedValue : CLEANUP_INTERVAL_DEFAULT_VALUE;
                }

                return this._cleanupIntervalInSeconds.Value;
            }
        }

        public string BaseAddress
        {
            get
            {
                if (this._baseAddress == null)
                {
                    this._baseAddress = string.Empty;
                    if (this._settingsDictionary.ContainsKey(BASE_ADDRESS_KEY))
                    {
                        _baseAddress = this._settingsDictionary[BASE_ADDRESS_KEY];
                    }
                }

                return this._baseAddress;
            }
        }
        #endregion

        #region Private Fields

        private readonly IDictionary<string, string> _settingsDictionary;

        private string _webuiBaseAddress;
        private string _webuiUser;
        private string _webuiPassword;

        private int? _webuiPort;
        private int? _cleanupIntervalInSeconds;

        private string _baseAddress;

        #endregion

        #region Constants

        public const string WEBUI_BASE_ADDRESS = "WebUiBaseAddress";
        public const string WEBUI_PORT = "WebUiPort";
        public const string WEBUI_USER = "WebUiUser";
        public const string WEBUI_PASSWORD = "WebUiPassword";
        public const string CLEANUP_INTERVAL = "CleanupIntervalInSeconds";
        public const string BASE_ADDRESS_KEY = "baseAddress";

        public const int WEBUI_PORT_DEFAULT_VALUE = 8080;
        public const int CLEANUP_INTERVAL_DEFAULT_VALUE = 600;

        #endregion
    }
}
