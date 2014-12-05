using Entities.Pattern;
using Repository.Pattern.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6.DataContext
{
    public class DataContext : DbContext, IDataContextAsync
    {
        private readonly Guid _instanceId;
        private bool _disposed;

        public DataContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            this._instanceId = Guid.NewGuid();
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public Guid InstanceId { get { return this._instanceId; } }

        public override int SaveChanges()
        {
            this.SyncObjectStatePreCommit();

            var changes = base.SaveChanges();

            this.SyncObjectStatePostCommit();

            return changes;
        }

        public override async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.SyncObjectStatePreCommit();

            var changesAsync = await base.SaveChangesAsync(cancellationToken);

            this.SyncObjectStatePostCommit();

            return changesAsync;
        }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState
        {
            this.Entry(entity).State = StateHelper.ConvertState(entity.ObjectState);
        }

        private void SyncObjectStatePreCommit()
        {
            foreach (var dbEntityEntry in this.ChangeTracker.Entries())
            {
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
            }
        }

        public void SyncObjectStatePostCommit()
        {
            foreach (var dbEntityEntry in this.ChangeTracker.Entries())
            {
                ((IObjectState)dbEntityEntry.Entity).ObjectState = StateHelper.ConvertState(dbEntityEntry.State);
            }
        }

        private void SyncObjectGraph(DbSet dbSet, object entity)
        {
            // Set tracking state for each child collections
            foreach (var prop in entity.GetType().GetProperties())
            {
                // Apply changes to 1-1 and N-1 properties
                var trackableRef = prop.GetValue(entity, null) as IObjectState;
                if (trackableRef != null && trackableRef.ObjectState == ObjectState.Added)
                {
                    dbSet.Attach(entity);
                    this.SyncObjectState((IObjectState)entity);
                }

                // Apply changes to 1-N properties
                var items = prop.GetValue(entity, null) as IList<IObjectState>;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        this.SyncObjectGraph(dbSet, item);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // free other managed objects that implement IDisposable
                }

                // release any unmanaged objects
                // set object references to null

                this._disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
