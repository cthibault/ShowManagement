using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Client.WPF.Infrastructure;
using ShowManagement.Client.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    abstract class BaseViewModel : ReactiveObject
    {
        protected BaseViewModel(IUnityContainer unityContainer)
        {
            this._unityContainer = unityContainer;
        }


        protected BusyContext AddBusyContext(string busyMessage)
        {
            var busyContext = new BusyContext(this.BusyContextManager, busyMessage);

            return busyContext;
        }


        protected IUnityContainer UnityContainer
        {
            get
            {
                if (this._unityContainer == null)
                {
                    this._unityContainer = App.UnityContainer;
                }
                return this._unityContainer;
            }
            set { this._unityContainer = value; }
        }
        private IUnityContainer _unityContainer;

        public BusyContextManager BusyContextManager
        {
            get
            {
                if (this._busyContextManager == null)
                {
                    this._busyContextManager = App.BusyContextManager;
                }
                return this._busyContextManager;
            }
            set { this._busyContextManager = value; }
        }
        private BusyContextManager _busyContextManager;
    }
}
