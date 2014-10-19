﻿using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    abstract class TrackableObject : BaseViewModel
    {
        protected TrackableObject()
            : this(null, true)
        {
        }
        protected TrackableObject(IUnityContainer unityContainer) 
            : this(unityContainer, true)
        {
        }
        protected TrackableObject(IUnityContainer unityContainer, bool trackChanges)
            : base(unityContainer)
        {
            this.TrackChanges = trackChanges;
            this.ChangesInternal.ChangeTrackingEnabled = trackChanges;

            this.HasChangesObservable.ToProperty(this, x => x.HasChanges, out this._hasChanges);
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
            this.ChangesInternal.ChangeTrackingEnabled = true;
        }

        /// <summary>
        /// Turn Track Changes Off and optionally clear existing change records
        /// </summary>
        /// <param name="clearChanges">Clear existing change records</param>
        public void TurnTrackChangesOff(bool clearChanges)
        {
            this.TrackChanges = false;

            if (clearChanges)
            {
                this.ClearChanges();
            }

            this.ChangesInternal.ChangeTrackingEnabled = false;
        }

        public void ClearChanges()
        {
            this.ChangesInternal.Clear();
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.HasChanges));
        }

        protected void LogChange(object oldValue, object newValue, [CallerMemberName] string propertyName = null)
        {
            Contract.Requires(propertyName != null);

            if (this.TrackChanges)
            {
                Change change = this.ChangesInternal.DefaultIfEmpty(Change.Default).SingleOrDefault(c => c.ValueName == propertyName);

                if (change.Equals(Change.Default))
                {
                    change = new Change(propertyName, oldValue, newValue);

                    this.ChangesInternal.Add(change);
                }
                else
                {
                    this.ChangesInternal.Remove(change);

                    if (!object.Equals(change.OriginalValue, newValue))
                    {
                        var newChange = new Change(propertyName, change.OriginalValue, newValue);
                        this.ChangesInternal.Add(change);
                    }
                }
            }
        }

        public bool TrackChanges { get; private set; }

        public IObservable<bool> HasChangesObservable
        {
            get
            {
                if (this._hasChangesObservable == null)
                {
                    this._hasChangesObservable = this.ChangesInternal.CountChanged.Select(x => x > 0);
                }
                return this._hasChangesObservable;
            }
        }
        private IObservable<bool> _hasChangesObservable;
        public bool HasChanges
        {
            get { return this.ChangesInternal.Any(); }
        }
        private readonly ObservableAsPropertyHelper<bool> _hasChanges;

        protected IReadOnlyCollection<Change> Changes
        {
            get { return new ReadOnlyCollection<Change>(this.ChangesInternal); }
        }

        private ReactiveList<Change> ChangesInternal = new ReactiveList<Change>();
    }
}
