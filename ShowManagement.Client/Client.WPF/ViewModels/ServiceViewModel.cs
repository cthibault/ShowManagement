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
    class ServiceViewModel : BaseViewModel
    {
        public ServiceViewModel(ServiceController serviceController)
        {
            if (serviceController == null)
            {
                throw new ArgumentNullException("serviceController");
            }

            this.ServiceController = serviceController;

            this.PopulateServiceDescription();

            this.CanStartObservable = this.WhenAnyValue(svm => svm.Status).Select(s => s == ServiceControllerStatus.Paused || s == ServiceControllerStatus.Stopped);
            this.CanStopObservable = this.WhenAnyValue(svm => svm.Status, svm => svm.CanStopInternal).Select(x => x.Item2 && x.Item1 == ServiceControllerStatus.Paused || x.Item1 == ServiceControllerStatus.Running);

            this.CanStartObservable.ToProperty(this, svm => svm.CanStart, out this._canStart);
            this.CanStopObservable.ToProperty(this, svm => svm.CanStop, out this._canStop);
        }


        public async Task<bool> Start()
        {
            bool started = false;

            if (this.CanStart)
            {
                await Task.Run(async () =>
                    {
                        try
                        {
                            this.ServiceController.Start();

                            await Task.Delay(1000);

                            this.Refresh();

                            started = true;
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    });
            }

            return started;
        }

        public async Task<bool> Stop()
        {
            bool stopped = false;

            if (this.CanStop)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        this.ServiceController.Stop();

                        await Task.Delay(1000);

                        this.Refresh();

                        stopped = true;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                });
            }

            return stopped;
        }


        private void PopulateServiceDescription()
        {
        }
        private void Refresh()
        {
            this.ServiceController.Refresh();

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.DisplayName));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Status));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.CanStopInternal));
        }


        public string DisplayName
        {
            get { return this.ServiceController.DisplayName; }
        }

        public string Description
        {
            get { return string.Empty; }
        }

        public ServiceControllerStatus Status
        {
            get { return this.ServiceController.Status; }
        }

        private bool CanStopInternal
        {
            get { return this.ServiceController.CanStop; }
        }
        
        
        public bool CanStart
        {
            get { return this._canStart.Value; }
        }
        public IObservable<bool> CanStartObservable { get; private set; }
        private ObservableAsPropertyHelper<bool> _canStart;

        public bool CanStop
        {
            get { return this._canStop.Value; }
        }
        public IObservable<bool> CanStopObservable { get; private set; }
        private ObservableAsPropertyHelper<bool> _canStop;

        public ServiceController ServiceController
        {
            get { return this._serviceController; }
            set { this.RaiseAndSetIfChanged(ref this._serviceController, value); }
        }
        private ServiceController _serviceController;
    }
}
