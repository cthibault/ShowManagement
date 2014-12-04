using Entities.Pattern;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6.Repository;
using Repository.Pattern.Repository;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6.UnitOfWork
{
    public class UnitOfWork : IUnitOfWorkAsync
    {
        private IDataContextAsync _dataContext;
        private bool _disposed;
        private ObjectContext _objectContext;
        private DbTransaction _transaction;


        public UnitOfWork(IDataContextAsync dataContext, IRepositoryProvider repositoryProvider)
        {
            this._dataContext = dataContext;

            this.RepositoryProvider = repositoryProvider;
            this.RepositoryProvider.DataContext = dataContext;
            this.RepositoryProvider.UnitOfWork = this;
        }


        public int SaveChanges()
        {
            return this._dataContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await this._dataContext.SaveChangesAsync(cancellationToken);
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            this._objectContext = ((IObjectContextAdapter)this._dataContext).ObjectContext;

            if (this._objectContext.Connection.State != ConnectionState.Open)
            {
                this._objectContext.Connection.Open();
            }

            this._transaction = this._objectContext.Connection.BeginTransaction(isolationLevel);
        }

        public bool Commit()
        {
            this._transaction.Commit();
            return true;
        }

        public void Rollback()
        {
            this._transaction.Rollback();
            this._dataContext.SyncObjectStatePostCommit();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IObjectState
        {
            return this.RepositoryAsync<TEntity>();
        }

        public IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IObjectState
        {
            return this.RepositoryProvider.GetRepositoryForEntityType<TEntity>();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    // Free other managed objects that implement IDisposable

                    try
                    {
                        if (this._objectContext != null && this._objectContext.Connection.State == ConnectionState.Open)
                        {
                            this._objectContext.Connection.Close();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // Do nothing, the ObjectContext has already been disposed
                    }

                    if (this._dataContext != null)
                    {
                        this._dataContext.Dispose();
                        this._dataContext = null;
                    }
                }

                // Releasee any unmanaged objects
                // Set the object references to null

                this._disposed = true;
            }
        }


        protected IRepositoryProvider RepositoryProvider { get; set; }
    }
}
