using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver
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
        public string ParentDirectory
        {
            get
            {
                if (this._parentDirectory == null)
                {
                    if (this._settingsDictionary.ContainsKey(PARENT_DIRECTORY_KEY))
                    {
                        this._parentDirectory = this._settingsDictionary[PARENT_DIRECTORY_KEY];
                    }
                    else
                    {
                        this._parentDirectory = PARENT_DIRECTORY_DEFAULT_VALUE;
                    }

                    string isRelativeSettingValue = string.Empty;
                    if (this._settingsDictionary.ContainsKey(RELATIVE_PARENT_DIRECTORY_KEY))
                    {
                        isRelativeSettingValue = this._settingsDictionary[RELATIVE_PARENT_DIRECTORY_KEY];

                        bool convertedValue = false;
                        if (bool.TryParse(isRelativeSettingValue, out convertedValue) && convertedValue)
                        {
                            this._parentDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this._parentDirectory);
                        }
                    }
                }

                return this._parentDirectory;
            }
        }

        public bool IncludeSubdirectories
        {
            get
            {
                if (!this._includeSubdirectories.HasValue)
                {
                    string settingValue = string.Empty;
                    if (this._settingsDictionary.ContainsKey(INCLUDE_SUBDIRECTORIES_KEY))
                    {
                        settingValue = this._settingsDictionary[INCLUDE_SUBDIRECTORIES_KEY];
                    }

                    bool convertedValue = false;
                    this._includeSubdirectories = bool.TryParse(settingValue, out convertedValue) && convertedValue;
                }
                return this._includeSubdirectories.Value;
            }
        }

        public bool InitialDirectoryScan
        {
            get
            {
                if (!this._initialDirectoryScan.HasValue)
                {
                    string settingValue = string.Empty;
                    if (this._settingsDictionary.ContainsKey(INITIAL_DIRECTORY_SCAN_KEY))
                    {
                        settingValue = this._settingsDictionary[INITIAL_DIRECTORY_SCAN_KEY];
                    }

                    bool convertedValue = false;
                    this._initialDirectoryScan = bool.TryParse(settingValue, out convertedValue) && convertedValue;
                }
                return this._initialDirectoryScan.Value;
            }
        }

        public IEnumerable<string> SupportedFileTypes
        {
            get
            {
                if (this._supportedFileTypes == null)
                {
                    if (this._settingsDictionary.ContainsKey(SUPPORTED_FILE_TYPES_KEY))
                    {
                        var settingsValue = this._settingsDictionary[SUPPORTED_FILE_TYPES_KEY];

                        this._supportedFileTypes = settingsValue.Split(',').ToList();
                    }

                    if (this._supportedFileTypes == null)
                    {
                        this._supportedFileTypes = new List<string>();
                    }
                }
                return this._supportedFileTypes;
            }
        }

        public int ItemRetryAttempts
        {
            get
            {
                if (!this._itemRetryAttempts.HasValue)
                {
                    string settingValue = string.Empty;
                    if (this._settingsDictionary.ContainsKey(ITEM_RETRY_ATTEMPTS_KEY))
                    {
                        settingValue = this._settingsDictionary[ITEM_RETRY_ATTEMPTS_KEY];
                    }

                    int convertedValue = 0;
                    this._itemRetryAttempts = int.TryParse(settingValue, out convertedValue) ? convertedValue : ITEM_RETRY_ATTEMPTS_DEFAULT_VALUE;
                }

                return this._itemRetryAttempts.Value;
            }
        }

        public int ItemRetryDurationSeconds
        {
            get
            {
                if (!this._itemRetryDurationSeconds.HasValue)
                {
                    string settingValue = string.Empty;
                    if (this._settingsDictionary.ContainsKey(ITEM_RETRY_DURATION_KEY))
                    {
                        settingValue = this._settingsDictionary[ITEM_RETRY_DURATION_KEY];
                    }

                    int convertedValue = 0;
                    this._itemRetryDurationSeconds = int.TryParse(settingValue, out convertedValue) ? convertedValue : ITEM_RETRY_DURATION_DEFAULT_VALUE;
                }

                return this._itemRetryDurationSeconds.Value;
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

        private string _parentDirectory;
        private bool? _includeSubdirectories;
        private bool? _initialDirectoryScan;
        private List<string> _supportedFileTypes;

        private int? _itemRetryAttempts;
        private int? _itemRetryDurationSeconds;

        private string _baseAddress;

        #endregion

        #region Constants

        public const string PARENT_DIRECTORY_KEY = "ParentDirectory";
        public const string RELATIVE_PARENT_DIRECTORY_KEY = "IsParentDirectoryRelative";
        public const string INCLUDE_SUBDIRECTORIES_KEY = "IncludeSubdirectories";
        public const string INITIAL_DIRECTORY_SCAN_KEY = "InitialDirectoryScan";
        public const string SUPPORTED_FILE_TYPES_KEY = "SupportedFileTypes";
        public const string ITEM_RETRY_ATTEMPTS_KEY = "ItemRetryAttempts";
        public const string ITEM_RETRY_DURATION_KEY = "ItemRetryDurationInSeconds";
        public const string BASE_ADDRESS_KEY = "baseAddress";

        public const string PARENT_DIRECTORY_DEFAULT_VALUE = "";
        public const int ITEM_RETRY_ATTEMPTS_DEFAULT_VALUE = 0;
        public const int ITEM_RETRY_DURATION_DEFAULT_VALUE = 0;

        #endregion
    }
}
