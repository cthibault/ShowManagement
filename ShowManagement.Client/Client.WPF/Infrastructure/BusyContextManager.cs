using ReactiveUI;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;

namespace ShowManagement.Client.WPF.Infrastructure
{
    public class BusyContextManager : ContextStore<string>
    {
        public BusyContextManager()
        {
            this.WhenAnyValue(bcm => bcm.Current)
                .Select(_ => this.Contexts.Any())
                .ToProperty(this, bcm => bcm.IsBusy, out this._isBusy);
        }

        public bool IsBusy
        {
            get { return this._isBusy.Value; }
        }
        readonly ObservableAsPropertyHelper<bool> _isBusy;
    }

    public class ContextStore<T> : ReactiveObject, IContextStore<T>
    {
        public ContextStore()
        {
        }

        public T Current
        {
            get
            {
                var currentData = default(T);

                AmbientContext<T> currentContext = this.CurrentContext;
                if (currentContext != null)
                {
                    currentData = currentContext.Data;
                }

                return currentData;
            }
        }
        
        protected AmbientContext<T> CurrentContext
        {
            get
            {
                var currentContext = this.Contexts.LastOrDefault();

                return currentContext;
            }
        }

        public void Add(AmbientContext<T> context)
        {
            this.Contexts.Add(context);

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Current));
        }
        public void Remove(AmbientContext<T> context)
        {
            AmbientContext<T> currentContext = this.CurrentContext;

            this.Contexts.Remove(context);

            if (context.Equals(currentContext))
            {
                this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Current));
            }
        }

        protected List<AmbientContext<T>> Contexts
        {
            get { return this._contexts; }
        }
        private List<AmbientContext<T>> _contexts = new List<AmbientContext<T>>();
    }
}
