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
            this._id = Guid.NewGuid();

            this._store = store;
            this._data = data;

            this._store.Add(this);
        }

        public Guid Id
        {
            get { return this._id; }
        }
        private readonly Guid _id;

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

        public override bool Equals(object obj)
        {
            return this.Equals(obj as AmbientContext<T>);
        }
        public bool Equals(AmbientContext<T> otherContext)
        {
            if (otherContext == null) return false;
            return this.Id.Equals(otherContext.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
