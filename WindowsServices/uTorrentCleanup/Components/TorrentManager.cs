using Microsoft.Practices.Unity;
using ShowManagement.Core.Extensions;
using ShowManagement.WindowsServices.uTorrentCleanup.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTorrentAPI;

namespace ShowManagement.WindowsServices.uTorrentCleanup.Components
{
    public class TorrentManager : BaseComponent, ITorrentManager
    {
        public TorrentManager(IUnityContainer unityContainer, Services.IServiceProvider serviceProvider, SettingsManager settingsManager)
            : base(unityContainer)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvicer");
            }

            this.SettingsManager = settingsManager;
            this.ServiceProvider = serviceProvider;
        }

        public async Task RemoveCompletedTorrents()
        {
            TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Verbose, 0, "Enter ShowManagement.WindowsServices.uTorrentCleanup.Components.TorrentManager.RemoveCompletedTorrents()");

            var saveShowDownloadsInfoTasks = new List<Task>();

            try
            {
                var client = new UTorrentClient(this.BuildWebUiUri(), this.SettingsManager.WebUiUser, this.SettingsManager.WebUiPassword);

                var completedTorrents = client.Torrents.Where(t => t.RemainingBytes == 0).ToList();

                for (int i = completedTorrents.Count - 1; i >= 0; i--)
                {
                    var torrentName = completedTorrents[i].Name;
                    var savePath = Path.Combine(completedTorrents[i].SavePath, completedTorrents[i].Name);

                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "Removed {0} from the Torrent Download Queue", torrentName);
                    client.Torrents.Remove(completedTorrents[i], TorrentRemovalOptions.TorrentFile);

                    TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Information, 0, "SavePath='{0}'", savePath);
                    var task = this.ServiceProvider.SaveShowDownloadInfo(savePath);
                    saveShowDownloadsInfoTasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                TraceSourceManager.TraceSource.TraceWithDateFormat(TraceEventType.Error, 0, "Exception caught in ShowManagement.WindowsServices.uTorrentCleanup.Components.TorrentManager.RemoveCompletedTorrents(): {0}", ex.ExtractExceptionMessage());
            }
            finally
            {
                Task.WaitAll(saveShowDownloadsInfoTasks.ToArray());
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
        private Services.IServiceProvider ServiceProvider { get; set; }
    }
}
