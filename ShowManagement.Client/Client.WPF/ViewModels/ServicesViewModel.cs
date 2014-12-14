using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Models;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Client.WPF.Services;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    interface IServicesViewModel
    {

    }

    class ServicesViewModel : BaseViewModel, IServicesViewModel
    {
        public ServicesViewModel(IUnityContainer unityContainer)
            : base(unityContainer)
        {
            // BusyContextManager just for this ViewModel
            this.BusyContextManager = new Infrastructure.BusyContextManager();
            
            this.DefineData();
            this.DefineCommands();
        }

        private void DefineData()
        {
            this.ServiceViewModels = this._serviceControllers.CreateDerivedCollection(sc => 
                {
                    var svm = new ServiceViewModel(sc);

                    return svm;
                }, null, (sc1, sc2) => sc1.DisplayName.CompareTo(sc2.DisplayName));
        }

        private void DefineCommands()
        {
            //TODO: Exception Handling?
            var selectedServiceCanStart = this.WhenAny(vm => vm.SelectedServiceViewModel, vm => vm.Force, (svm, force) => svm.GetValue() != null && svm.Value.CanStart);
            var selectedServiceCanStop = this.WhenAny(vm => vm.SelectedServiceViewModel, vm => vm.Force, (svm, force) => svm.GetValue() != null && svm.Value.CanStop);

            this.StartServiceCommand = ReactiveCommand.Create(selectedServiceCanStart);
            this.StartServiceCommand.Subscribe(async _ => await this.OnStartService());

            this.StopServiceCommand = ReactiveCommand.Create(selectedServiceCanStop);
            this.StopServiceCommand.Subscribe(async _ => await this.OnStopService());

            this.RefreshServicesCommand = ReactiveCommand.CreateAsyncTask(async x => await this.OnRefreshServices());
            this.RefreshServicesCommand.Subscribe(async result => await this.OnRefreshServicesComplete(result));
            this.RefreshServicesCommand.ThrownExceptions.Subscribe(ex => { throw ex; });
        }

        #region StartService
        public ReactiveCommand<object> StartServiceCommand { get; private set; }
        private async Task OnStartService()
        {
            if (this.SelectedServiceViewModel != null)
            {
                using (this.AddBusyContext(string.Format("Starting {0}...", this.SelectedServiceViewModel.DisplayName)))
                {
                    await this.SelectedServiceViewModel.Start();

                    this.ForceReevaluation();
                }
            }
        }
        #endregion

        #region StopService
        public ReactiveCommand<object> StopServiceCommand { get; private set; }
        private async Task OnStopService()
        {
            if (this.SelectedServiceViewModel != null)
            {
                using (this.AddBusyContext(string.Format("Stopping {0}...", this.SelectedServiceViewModel.DisplayName)))
                {
                    await this.SelectedServiceViewModel.Stop();

                    this.ForceReevaluation();
                }
            }
        }
        #endregion

        #region RefreshServices
        public ReactiveCommand<Tuple<List<ServiceController>, BusyContext>> RefreshServicesCommand { get; private set; }

        private async Task<Tuple<List<ServiceController>, BusyContext>> OnRefreshServices()
        {
            var busyContext = this.AddBusyContext("Searching for Show Management services...");

            this._serviceControllers.Clear();

            List<ServiceController> myServices = ServiceController.GetServices().Where(sc => sc.ServiceName.StartsWith("SM.")).ToList();

            await Task.Delay(1000);

            return new Tuple<List<ServiceController>,BusyContext>(myServices, busyContext);
        }
        private async Task OnRefreshServicesComplete(Tuple<List<ServiceController>, BusyContext> results)
        {
            if (results != null)
            {
                using (results.Item2)
                {
                    using (var context = this.AddBusyContext("Populating Show Management services..."))
                    {
                        this._serviceControllers.Clear();

                        if (results.Item1 != null)
                        {
                            this._serviceControllers.AddRange(results.Item1);
                        }
                    }
                }
            }
        }
        #endregion


        #region ForceReevaluation
        /// <summary>
        /// This function is to force the Observables and Bindings for the Start and Stop commands
        /// to reevaluate their state.  I'm having problems getting the change in the ServiceViewModel
        /// to trigger this reevaluation.
        /// </summary>
        private void ForceReevaluation()
        {
            this.Force = !this.Force;
        }
        private bool Force
        {
            get { return this._force; }
            set { this.RaiseAndSetIfChanged(ref this._force, value); }
        }
        private bool _force; 
        #endregion

        public IReactiveDerivedList<ServiceViewModel> ServiceViewModels { get; private set; }
        private ReactiveList<ServiceController> _serviceControllers = new ReactiveList<ServiceController>();

        public ServiceViewModel SelectedServiceViewModel
        {
            get { return this._selectedServiceViewModel; }
            set { this.RaiseAndSetIfChanged(ref this._selectedServiceViewModel, value); }
        }
        private ServiceViewModel _selectedServiceViewModel;
    }
}
