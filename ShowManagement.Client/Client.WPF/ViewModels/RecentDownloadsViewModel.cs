using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Models;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Client.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    interface IRecentDownloadsViewModel
    {

    }

    class RecentDownloadsViewModel : TrackableObject, IRecentDownloadsViewModel
    {
        public RecentDownloadsViewModel(IUnityContainer unityContainer, Services.IRecentDownloadsServiceProvider serviceProvider)
            : base(unityContainer)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // BusyContextManager just for this ViewModel
            this.BusyContextManager = new Infrastructure.BusyContextManager();

            this.ServiceProvider = serviceProvider;

            this.DefineData();
            this.DefineCommands();
        }

        private void DefineData()
        {
            this.HasChangesObservable.ToProperty(this, x => x.NeedsToBeRefreshed, out this._needsToBeRefreshed);

            this.VisibleSearchResults = this._searchResults.CreateDerivedCollection(
                sdi => new RecentDownloadViewModel(sdi),
                sdi => sdi.CreatedDate.HasValue && sdi.ModifiedDate.HasValue,
                (rdvm1, rdvm2) => rdvm2.Model.CreatedDate.Value.CompareTo(rdvm1.Model.CreatedDate.Value));

            this.StartDate = DateTime.Now.Date.AddDays(-14);
        }

        private void DefineCommands()
        {
            //TODO: Exception Handling?

            this.RefreshCommand = ReactiveCommand.CreateAsyncTask(async x => await this.OnRefresh());
            this.RefreshCommand.Subscribe(async results => await this.OnRefreshComplete(results));
        }

        #region Search
        public ReactiveCommand<Tuple<List<ShowDownloadInfo>, BusyContext>> RefreshCommand { get; private set; }

        private async Task<Tuple<List<ShowDownloadInfo>, BusyContext>> OnRefresh()
        {
            List<ShowDownloadInfo> results = null;

            this.IsEnabled = false;

            BusyContext busyContext = null;

            if (this.StartDate.HasValue)
            {
                busyContext = this.AddBusyContext(string.Format("Refreshing Recent Downloads as of {0}...", this.StartDate.Value.ToShortDateString()));

                results = await this.ServiceProvider.GetRecentDownloads(this.StartDate.Value);
            }
            else
            {
                busyContext = this.AddBusyContext("Refreshing Recent Downloads...");

                results = await this.ServiceProvider.GetRecentDownloads();
            }

            return new Tuple<List<ShowDownloadInfo>, BusyContext>(results, busyContext);
        }
        private async Task OnRefreshComplete(Tuple<List<ShowDownloadInfo>, BusyContext> results)
        {
            if (results != null)
            {
                using (results.Item2)
                {
                    using (var context = this.AddBusyContext("Populating Recent Downloads..."))
                    {
                        this._searchResults.Clear();

                        if (results.Item1 != null)
                        {
                            this._searchResults.AddRange(results.Item1);
                        }

                        this.ClearChanges();
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("results");
            }

            this.IsEnabled = true;
        }
        #endregion


        public DateTime? StartDate
        {
            get { return this._startDate; }
            set { this.LogRaiseAndSetIfChanged(this._startDate, value, v => this._startDate = v); }
        }
        private DateTime? _startDate;

        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set { this.RaiseAndSetIfChanged(ref this._isEnabled, value); }
        }
        private bool _isEnabled;

        public bool NeedsToBeRefreshed
        {
            get { return this._needsToBeRefreshed.Value; }
        }
        private ObservableAsPropertyHelper<bool> _needsToBeRefreshed;


        public IReactiveDerivedList<RecentDownloadViewModel> VisibleSearchResults { get; private set; }
        private ReactiveList<ShowDownloadInfo> _searchResults = new ReactiveList<ShowDownloadInfo>();


        private IRecentDownloadsServiceProvider ServiceProvider { get; set; }
    }
}
