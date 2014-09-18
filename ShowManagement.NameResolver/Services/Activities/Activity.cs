using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.NameResolver.Services.Activities
{
    abstract class Activity : IActivity
    {
        protected Activity(int maxRetryAttempts)
        {
            this._maxRetryAttempts = maxRetryAttempts;
            this._createdDtm = DateTime.Now;
        }

        public abstract Task<IActivity> Perform();

        public virtual async Task Cancel()
        {
            this.IsCancelled = true;
        }

        public abstract bool Equals(IActivity other);
        public override bool Equals(object obj)
        {
            var convertedObj = obj as IActivity;

            return this.Equals(convertedObj);
        }

        public override int GetHashCode()
        {
            return this.GetHashCodeImplementation();
        }
        protected abstract int GetHashCodeImplementation();


        public int MaxRetryAttempts
        {
            get { return this._maxRetryAttempts; }
        }

        public DateTime CreatedDtm
        {
            get { return this._createdDtm; }
        }

        public bool IsCancelled { get; protected set; }


        private readonly int _maxRetryAttempts;
        private readonly DateTime _createdDtm;
    }
}
