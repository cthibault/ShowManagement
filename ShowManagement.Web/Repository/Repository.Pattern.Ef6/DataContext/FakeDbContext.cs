using Entities.Pattern;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6.DataContext
{
    public abstract class FakeDbContext : IFakeDbContext
    {
        private readonly Dictionary<Type, object> _fakeDbSets;

        protected FakeDbContext()
        {
            this._fakeDbSets = new Dictionary<Type, object>();
        }

        public int SaveChanges()
        {
            return default(int);
        }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState
        {
            // Intentionally Left empty
        }

        public Task<int> SaveChangesAsync()
        {
            return new Task<int>(() => default(int));
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return new Task<int>(() => default(int));
        }

        public void Dispose()
        {

        }

        public DbSet<T> Set<T>() where T : class
        {
            return (DbSet<T>)this._fakeDbSets[typeof(T)];
        }

        public void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : Entity, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new()
        {
            var fakeDbSet = Activator.CreateInstance<TFakeDbSet>();
            this._fakeDbSets.Add(typeof(TEntity), fakeDbSet);
        }
        
        public void SyncObjectStatePostCommit()
        {
            
        }
    }
}
