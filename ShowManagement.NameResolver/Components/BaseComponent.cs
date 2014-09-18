using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Components
{
    public abstract class BaseComponent
    {
        protected BaseComponent(IUnityContainer unityContainer)
        {
            this._unityContainer = unityContainer;
        }

        #region Public Properties
        public IUnityContainer UnityContainer
        {
            get { return this._unityContainer; }
        }
        #endregion

        #region Private Fields
        private readonly IUnityContainer _unityContainer;
        #endregion
    }
}
