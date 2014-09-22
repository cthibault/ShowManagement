using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Components.Activities
{
    public interface IActivity: IEquatable<IActivity>
    {
        Task<IActivity> Perform();

        Task Cancel();

        int MaxRetryAttempts { get; }
        DateTime CreatedDtm { get; }
        bool IsCancelled { get; }
    }
}
