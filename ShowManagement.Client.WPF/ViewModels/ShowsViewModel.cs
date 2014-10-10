using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Models;
using ShowManagement.Client.WPF.Models;
using ShowManagement.CommonServiceProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    class ShowsViewModel : BaseViewModel
    {
        public ShowsViewModel(IUnityContainer unityContainer, Services.IServiceProvider serviceProvider)
            : base(unityContainer)
        {
            this.ServiceProvider = serviceProvider;

            this.DefineData();
            this.DefineCommands();
        }

        private void DefineData()
        {
            this.Shows = this.ShowModels.CreateDerivedCollection(
                s => 
                    {
                        var vm = new ShowViewModel(this.UnityContainer, this.ServiceProvider, this.BusyContextManager, s);

                        vm.SelectedCommand.Subscribe(_ => this.SelectedShowViewModel = vm);
                        vm.CloseCommand.Subscribe(_ => this.SelectedShowViewModel = null);

                        return vm;
                    },
                    null, 
                    (s1, s2) => s1.Name.CompareTo(s2.Name));

            this.ShowsToSave = this.Shows.CreateDerivedCollection(svm => svm, svm => svm.NeedsToBeSaved);

            this.WhenAnyValue(vm => vm.SelectedShowViewModel)
                .Select(x => x != null)
                .ToProperty(this, vm => vm.ShowEditPane, out this._showEditPane);
        }
        private void DefineCommands()
        {
            #region Refresh Shows
            this.RefreshShowsCommand = ReactiveCommand.CreateAsyncTask(async x => await this.OnRefreshShows());
            this.RefreshShowsCommand.Subscribe(async results => await this.OnRefreshShowsComplete(results));
            this.RefreshShowsCommand.ThrownExceptions.Subscribe(ex => { throw ex; }); 
            #endregion

            #region Add
            this.AddShowCommand = ReactiveCommand.Create(this.RefreshShowsCommand.CanExecuteObservable);
            this.AddShowCommand.Subscribe(async _ => await this.OnAddShow());
            this.AddShowCommand.ThrownExceptions.Subscribe(ex => { throw ex; });
            #endregion

            #region Save All
            this.SaveShowsCommand = ReactiveCommand.CreateAsyncTask(
                this.ShowsToSave.CountChanged.Select(x => x > 0),
                async x => await this.OnSaveShows());
            this.SaveShowsCommand.ThrownExceptions.Subscribe(ex => { throw ex; });
            #endregion
        }


        #region OnRefreshShows
        private async Task<Tuple<List<ShowInfo>, BusyContext>> OnRefreshShows()
        {
            List<ShowInfo> results = null;

            var busyContext = this.AddBusyContext("Retrieving Shows...");

            results = await this.ServiceProvider.GetAllShows();

            return new Tuple<List<ShowInfo>, BusyContext>(results, busyContext);
        }
        private async Task OnRefreshShowsComplete(Tuple<List<ShowInfo>, BusyContext> results)
        {
            if (results != null)
            {
                using (results.Item2)
                {
                    using (var context = this.AddBusyContext("Populating Shows List..."))
                    {
                        using (this.ShowModels.SuppressChangeNotifications())
                        {
                            this.ShowModels.Clear();

                            if (results.Item1 != null)
                            {
                                this.ShowModels.AddRange(results.Item1);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("results");
            }
        } 
        #endregion

        #region OnAddShow
        private async Task OnAddShow()
        {
            var count = this.Shows.Count(svm => svm.Name.StartsWith(NEWSHOW_NAME));

            string showName = count == 0 ? NEWSHOW_NAME : string.Format("{0} ({1})", NEWSHOW_NAME, ++count);

            this.ShowModels.Add(new ShowInfo { Name = showName });
        }
        #endregion

        #region OnSaveShows
        private async Task OnSaveShows()
        {
            using (var context = this.AddBusyContext("Saving..."))
            {
                await Task.Delay(10000);
            }
        }
        #endregion



        #region Commands
        public ReactiveCommand<Tuple<List<ShowInfo>, BusyContext>> RefreshShowsCommand { get; private set; }
        public ReactiveCommand<object> AddShowCommand { get; private set; }
        public ReactiveCommand<Unit> SaveShowsCommand { get; private set; }
        #endregion


        public IReactiveDerivedList<ShowViewModel> Shows { get; private set; }
        private ReactiveList<ShowInfo> ShowModels = new ReactiveList<ShowInfo>();

        private IReactiveDerivedList<ShowViewModel> ShowsToSave { get; set; }

        public ShowViewModel SelectedShowViewModel
        {
            get { return this._selectedShowViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._selectedShowViewModel, value); }
        }
        private ShowViewModel _selectedShowViewModel;

        public bool ShowEditPane
        {
            get { return this._showEditPane.Value; }
        }
        private ObservableAsPropertyHelper<bool> _showEditPane;

        #region ShowProvider
        private Services.IServiceProvider ServiceProvider { get; set; }
        #endregion

        #region Constants
        private const string NEWSHOW_NAME = "_New Show";

        public static readonly string TitleText = "shows";
        #endregion
    }
}
