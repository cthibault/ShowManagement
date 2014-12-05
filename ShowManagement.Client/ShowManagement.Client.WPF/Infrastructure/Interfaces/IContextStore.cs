using ShowManagement.Client.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.Infrastructure
{
    public interface IContextStore<T>
    {
        T Current { get; }
        void Add(AmbientContext<T> context);
        void Remove(AmbientContext<T> context);
    }
}
