using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Infrastructure
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

        private string _baseAddress;

        #endregion

        #region Constants

        public const string BASE_ADDRESS_KEY = "baseAddress";

        #endregion
    }
}
