using ReactiveUI;
using ShowManagement.Client.WPF.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    abstract class TrackableObject : ReactiveObject
    {
        protected TrackableObject()
            : this(true)
        {
        }
        protected TrackableObject(bool trackChanges)
        {
            this.TrackChanges = trackChanges;
        }

        protected T LogRaiseAndSetIfChanged<T>(T oldValue, T newValue, Action<T> setAction, [CallerMemberName] string propertyName = null)
        {
            Contract.Requires(propertyName != null);
            Contract.Requires(setAction != null);

            if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                this.RaisePropertyChanging(propertyName);
                setAction(newValue);
                this.LogChange(oldValue, newValue, propertyName);
                this.RaisePropertyChanged(propertyName);
            }

            return newValue;
        }

        /// <summary>
        /// Turn Track Changes On
        /// </summary>
        public void TurnTrackChangesOn()
        {
            this.TrackChanges = true;
        }

        /// <summary>
        /// Turn Track Changes Off and optionally clear existing change records
        /// </summary>
        /// <param name="clearChanges">Clear existing change records</param>
        public void TurnTrackChangesOff(bool clearChanges)
        {
            this.TrackChanges = false;

            this.ChangesInternal.Clear();
        }

        protected void LogChange(object oldValue, object newValue, [CallerMemberName] string propertyName = null)
        {
            Contract.Requires(propertyName != null);

            if (this.TrackChanges)
            {
                Change change;

                if (!this.ChangesInternal.TryGetValue(propertyName, out change))
                {
                    change = new Change(propertyName, oldValue, newValue);

                    this.ChangesInternal.Add(propertyName, change);
                }
                else
                {
                    if (object.Equals(change.OriginalValue, newValue))
                    {
                        this.ChangesInternal.Remove(propertyName);
                    }
                    else
                    {
                        var newChange = new Change(propertyName, change.OriginalValue, newValue);

                        this.ChangesInternal[propertyName] = newChange;
                    }
                }
            }
        }

        public bool TrackChanges { get; protected set; }

        public bool HasChanges
        {
            get { return this.ChangesInternal.Any(); }
        }

        protected IEnumerable<Change> Changes
        {
            get { return this.ChangesInternal.Values; }
        }

        private Dictionary<string, Change> ChangesInternal = new Dictionary<string, Change>();
    }
}
