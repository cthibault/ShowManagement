using Microsoft.Practices.Unity;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
