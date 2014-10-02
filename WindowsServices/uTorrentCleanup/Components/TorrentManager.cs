using Microsoft.Practices.Unity;
using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.uTorrentCleanup.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTorrentAPI;

namespace ShowManagement.WindowsServices.uTorrentCleanup.Components
{
    public class TorrentManager : BaseComponent, ITorrentManager
    {
        public TorrentManager(IUnityContainer unityContainer, SettingsManager settingsManager)
            : base(unityContainer)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            this.SettingsManager = settingsManager;
        }

        public async Task RemoveCompletedTorrents()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Components.TorrentManager.RemoveCompletedTorrents()");

            try
            {
                var client = new UTorrentClient(this.BuildWebUiUri(), this.SettingsManager.WebUiUser, this.SettingsManager.WebUiPassword);

                var completedTorrents = client.Torrents.Where(t => t.RemainingBytes == 0).ToList();

                for (int i = completedTorrents.Count - 1; i >= 0; i--)
                {
                    var torrentName = completedTorrents[i].Name;

                    client.Torrents.Remove(completedTorrents[i], TorrentRemovalOptions.TorrentFile);
                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Removed {0} from the Torrent Download Queue", torrentName);
                }
            }
            catch (Exception ex)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ShowManagement.WindowsServices.uTorrentCleanup.Components.TorrentManager.RemoveCompletedTorrents(): {0}", ex.ExtractExceptionMessage());
            }

            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Components.TorrentManager.RemoveCompletedTorrents()");
        }

        private Uri BuildWebUiUri()
        {
            var builder = new UriBuilder();

            builder.Host = this.SettingsManager.WebUiBaseAddress;
            builder.Port = this.SettingsManager.WebUiPort;
            builder.Path = "gui";

            return builder.Uri;
        }


        private SettingsManager SettingsManager { get; set; }
    }
}
