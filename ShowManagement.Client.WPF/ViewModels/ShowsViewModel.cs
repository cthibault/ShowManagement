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

            // Load Initial Data
            //this.RefreshShowsCommand.Execute(null);
        }

        private void DefineData()
        {
            this.Shows = this.ShowModels.CreateDerivedCollection(
                s => 
                    {
                        var vm = new ShowViewModel(s);

                        //vm.SelectedCommand.Subscribe(_ => vm.Name = vm.Name + " Curtis");

                        return vm;
                    },
                    null, 
                    (s1, s2) => s1.Name.CompareTo(s2.Name));

            this.ShowsToSave = this.Shows.CreateDerivedCollection(svm => svm, svm => svm.NeedsToBeSaved);
        }
        private void DefineCommands()
        {
            #region Add
            this.AddShowCommand = ReactiveCommand.Create();
            this.AddShowCommand.Subscribe(_ =>
                {
                    var count = this.Shows.Count(svm => svm.Name.StartsWith(NEWSHOW_NAME));

                    string showName = count == 0 ? NEWSHOW_NAME : string.Format("{0} ({1})", NEWSHOW_NAME, ++count);

                    this.ShowModels.Add(new ShowInfo { Name = showName });
                }); 
            #endregion

            #region Refresh Shows
            this.RefreshShowsCommand = ReactiveCommand.CreateAsyncTask(async x =>
                    {
                        List<ShowInfo> results = null;

                        var busyContext = this.AddBusyContext("Retrieving Shows...");

                        results = await this.ServiceProvider.GetAllShows();

                        return new Tuple<List<ShowInfo>, BusyContext>(results, busyContext);
                    });
            this.RefreshShowsCommand.Subscribe(results =>
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
                });
            this.RefreshShowsCommand.ThrownExceptions.Subscribe(ex => { throw ex; }); 
            #endregion

            #region Save All
            this.SaveAllCommand = ReactiveCommand.CreateAsyncTask(
                this.ShowsToSave.CountChanged.Select(x => x > 0),
                async x =>
                {
                    using (var context = this.AddBusyContext("Saving..."))
                    {
                        await Task.Delay(10000);
                    }
                });
            #endregion
        }

        #region Commands
        public ReactiveCommand<Tuple<List<ShowInfo>, BusyContext>> RefreshShowsCommand { get; private set; }
        public ReactiveCommand<object> AddShowCommand { get; private set; }
        public ReactiveCommand<Unit> SaveAllCommand { get; private set; }
        #endregion


        public IReactiveDerivedList<ShowViewModel> Shows { get; private set; }
        private ReactiveList<ShowInfo> ShowModels = new ReactiveList<ShowInfo>();

        private IReactiveDerivedList<ShowViewModel> ShowsToSave { get; set; }

        #region ShowProvider
        private Services.IServiceProvider ServiceProvider { get; set; }
        #endregion

        #region Constants
        private const string NEWSHOW_NAME = "_New Show";

        public static readonly string TitleText = "shows";
        #endregion
    }
}
