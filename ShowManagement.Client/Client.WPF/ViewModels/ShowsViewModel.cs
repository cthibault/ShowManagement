using Entities.Pattern;
using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Models;
using ShowManagement.Client.WPF.Infrastructure;
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
    interface IShowsViewModel
    {

    }
    class ShowsViewModel : BaseViewModel, IShowsViewModel
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
            this.AllShowViewModels = this.ShowModels.CreateDerivedCollection(
                s =>
                {
                    var vm = new ShowViewModel(this.UnityContainer, this.ServiceProvider, this.BusyContextManager, s);

                    vm.SelectedCommand.Subscribe(_ => this.SelectedShowViewModel = vm);
                    vm.CloseCommand.Subscribe(_ => this.SelectedShowViewModel = null);
                    vm.DeleteCommand.Subscribe(_ =>
                    {
                        if (vm.IsNew)
                        {
                            this.ShowModels.Remove(vm.Model);
                        }
                    });
                    vm.SaveCommand.Subscribe(async _ =>
                    {
                        // Close the EditPanel
                        this.SelectedShowViewModel = null;

                        var results = await this.OnSaveShows(new List<ShowViewModel> { vm });
                        await this.OnSaveShowsComplete(results);
                    });

                    return vm;
                });

            this.AllShowViewModels.ChangeTrackingEnabled = true;

            this.VisibleShowViewModels = this.AllShowViewModels.CreateDerivedCollection(svm => svm, svm => svm.ObjectState != ObjectState.Deleted, (s1, s2) => s1.Name.CompareTo(s2.Name));
            this.ShowsToSave = this.AllShowViewModels.CreateDerivedCollection(svm => svm, svm => svm.NeedsToBeSaved);

            this.NeedsToBeSavedObservable = this.ShowsToSave.CountChanged.Select(x => x > 0);
            this.NeedsToBeSavedObservable.ToProperty(this, vm => vm.NeedsToBeSaved, out this._needsToBeSaved);

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
            this.SaveShowsCommand = ReactiveCommand.CreateAsyncTask(this.NeedsToBeSavedObservable, async _ => await this.OnSaveShows(this.ShowsToSave.ToList()));
            this.SaveShowsCommand.Subscribe(async results => await this.OnSaveShowsComplete(results));
            this.SaveShowsCommand.ThrownExceptions.Subscribe(ex => { throw ex; });
            #endregion
        }

        #region RefreshShows
        public ReactiveCommand<Tuple<List<ShowInfo>, BusyContext>> RefreshShowsCommand { get; private set; }

        private async Task<Tuple<List<ShowInfo>, BusyContext>> OnRefreshShows()
        {
            List<ShowInfo> results = null;

            var busyContext = this.AddBusyContext("Retrieving Shows...");

            this.SelectedShowViewModel = null;

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

        #region AddShow
        public ReactiveCommand<object> AddShowCommand { get; private set; }

        private async Task OnAddShow()
        {
            var count = this.VisibleShowViewModels.Count(svm => svm.Name.StartsWith(ShowViewModel.NEWSHOW_NAME));

            string showName = count == 0 ? ShowViewModel.NEWSHOW_NAME : string.Format("{0} ({1})", ShowViewModel.NEWSHOW_NAME, ++count);

            var showInfo = new ShowInfo { ObjectState = ObjectState.Added };
            this.ShowModels.Add(showInfo);

            var viewModel = this.VisibleShowViewModels.First(vm => vm.Model.Equals(showInfo));

            // Set Name on the ViewModel for property changed handling
            viewModel.Name = showName;

            viewModel.SelectedCommand.Execute(null);
        }
        #endregion

        #region SaveShows
        public ReactiveCommand<Tuple<List<ShowInfo>, List<ShowInfo>, BusyContext>> SaveShowsCommand { get; private set; }

        private async Task<Tuple<List<ShowInfo>, List<ShowInfo>, BusyContext>> OnSaveShows(List<ShowViewModel> showViewModels)
        {
            List<ShowInfo> results = null;

            List<ShowInfo> modelsToSave = null;

            var busyContext = this.AddBusyContext("Saving...");

            if (showViewModels != null)
            {
                modelsToSave = new List<ShowInfo>(showViewModels.Count);

                foreach (var svm in showViewModels)
                {
                    svm.IsEnabled = false;

                    if (svm == this.SelectedShowViewModel)
                    {
                        this.SelectedShowViewModel = null;
                    }

                    modelsToSave.Add(svm.Model);
                }

                results = await this.ServiceProvider.SaveShows(modelsToSave);
            }

            return new Tuple<List<ShowInfo>, List<ShowInfo>, BusyContext>(modelsToSave, results, busyContext);
        }
        private async Task OnSaveShowsComplete(Tuple<List<ShowInfo>, List<ShowInfo>, BusyContext> results)
        {
            if (results != null)
            {
                using (results.Item3)
                {
                    using (var context = this.AddBusyContext("Updating saved shows..."))
                    {
                        if (results.Item1 != null)
                        {
                            this.ShowModels.RemoveAll(results.Item1);
                        }

                        if (results.Item2 != null)
                        {
                            this.ShowModels.AddRange(results.Item2);
                        }
                    }
                }
            }
        }
        #endregion


        public IReactiveDerivedList<ShowViewModel> AllShowViewModels { get; private set; }
        public IReactiveDerivedList<ShowViewModel> VisibleShowViewModels { get; private set; }
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

        public bool NeedsToBeSaved
        {
            get { return this._needsToBeSaved.Value; }
        }
        private ObservableAsPropertyHelper<bool> _needsToBeSaved;
        private IObservable<bool> NeedsToBeSavedObservable { get; set; }

        #region ServiceProvider
        private Services.IServiceProvider ServiceProvider
        {
            get
            {
                if (this._serviceProvider == null)
                {
                    this._serviceProvider = this.UnityContainer.Resolve<Services.IServiceProvider>();
                }
                return this._serviceProvider;
            }
            set { this._serviceProvider = value; }
        }
        private Services.IServiceProvider _serviceProvider;
        #endregion

        #region Constants
        public static readonly string TitleText = "shows";
        #endregion
    }
}
