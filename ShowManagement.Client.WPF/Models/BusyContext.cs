using ShowManagement.Client.WPF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Models
{
    public class BusyContext : AmbientContext<string>
    {
        public BusyContext(IContextStore<string> store, string message)
            : base(store, message)
        {
        }
    }
    public abstract class AmbientContext<T> : IDisposable
    {
        public AmbientContext(IContextStore<T> store, T data)
        {
            this._store = store;
            this._data = data;

            this._store.Add(this);
        }

        public T Data
        {
            get { return this._data; }
        }
        private readonly T _data;

        private readonly IContextStore<T> _store;

        public void Dispose()
        {
            this._store.Remove(this);
        }
    }
}
