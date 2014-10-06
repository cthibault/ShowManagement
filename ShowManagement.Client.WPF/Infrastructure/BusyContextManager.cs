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
                var current = default(T);

                if (this.Contexts.Any())
                {
                    current = this.Contexts.Peek().Data;
                }

                return current;
            }
        }

        public void Add(AmbientContext<T> context)
        {
            this.Contexts.Push(context);

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Current));
        }
        public void Remove(AmbientContext<T> context)
        {
            var currentContext = this.Contexts.Pop();

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Current));

#if DEBUG
            if (!ReferenceEquals(currentContext, context))
            {
                throw new InvalidOperationException("Context Store - The removed context does not match the expected context");
            } 
#endif
        }

        protected Stack<AmbientContext<T>> Contexts
        {
            get { return this._contexts; }
        }
        private Stack<AmbientContext<T>> _contexts = new Stack<AmbientContext<T>>();
    }
}
