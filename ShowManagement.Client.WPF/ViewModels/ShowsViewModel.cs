using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Models;
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
        public ShowsViewModel(IUnityContainer unityContainer)
            : base(unityContainer)
        {
            this.InitializeCommands();
            this.InitializeData();
        }

        private void InitializeData()
        {
            this.Shows = this.ShowModels.CreateDerivedCollection(
                s => new ShowViewModel(s),
                null,
                (s1, s2) => s1.Name.CompareTo(s2.Name));

        }
        private void InitializeCommands()
        {
            this.AddShowCommand = ReactiveCommand.CreateAsyncTask(x => this.AddShow());
            this.RefreshShowsCommand = ReactiveCommand.CreateAsyncTask(x => this.RefreshShows());
        }

        #region Refresh
        public ReactiveCommand<Unit> RefreshShowsCommand { get; private set; }
        private async Task RefreshShows()
        {
            try
            {
                var service = new ShowManagement.Client.WPF.Services.ServiceProvider();

                var newData = await service.GetAllShows();

                this.ShowModels.Clear();

                this.ShowModels.AddRange(newData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        #region Add
        public ReactiveCommand<Unit> AddShowCommand { get; private set; }
        private async Task AddShow()
        {
            this.ShowModels.Add(new ShowInfo() { Name = "Add" });
        }
        #endregion

        #region Select

        #endregion


        private ReactiveList<ShowInfo> ShowModels = new ReactiveList<ShowInfo>();
        public IReactiveDerivedList<ShowViewModel> Shows { get; private set; }

        #region Constants
        public static readonly string TitleText = "shows";
        #endregion
    }
}
