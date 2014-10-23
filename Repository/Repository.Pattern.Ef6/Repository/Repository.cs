using Entities.Pattern;
using LinqKit;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6.DataContext;
using Repository.Pattern.Repository;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6.Repository
{
    public class Repository<TEntity> : IRepositoryAsync<TEntity> 
        where TEntity : class, IObjectState
    {
        private readonly IDataContextAsync _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public Repository(IDataContextAsync context, IUnitOfWorkAsync unitOfWork)
        {
            this._context = context;
            this._unitOfWork = unitOfWork;

            var dbContext = context as DbContext;
            if (dbContext != null)
            {
                this._dbSet = dbContext.Set<TEntity>();
            }
            else
            {
                var fakeContext = context as FakeDbContext;
                if (fakeContext != null)
                {
                    this._dbSet = fakeContext.Set<TEntity>();
                }
            }
        }


        public virtual TEntity Find(params object[] keyValues)
        {
            return this._dbSet.Find(keyValues);
        }

        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await this.FindAsync(CancellationToken.None, keyValues);
        }

        public async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await this._dbSet.FindAsync(cancellationToken, keyValues);
        }


        public virtual void Insert(TEntity entity)
        {
            entity.ObjectState = ObjectState.Added;
            this._dbSet.Attach(entity);
            this._context.SyncObjectState(entity);
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                this.Insert(entity);
            }
        }

        public void InsertOrUpdateGraph(TEntity entity)
        {
            this.SyncObjectGraph(entity);
            this._dbSet.Add(entity);
        }

        public void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            this._dbSet.AddRange(entities);
        }


        public void Delete(object id)
        {
            var entity = this._dbSet.Find(id);
            this.Delete(entity);
        }

        public void Delete(TEntity entity)
        {
            entity.ObjectState = ObjectState.Deleted;
            this._dbSet.Attach(entity);
            this._context.SyncObjectState(entity);
        }

        public async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await this.DeleteAsync(CancellationToken.None, keyValues);
        }

        public async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await this.FindAsync(cancellationToken, keyValues);

            bool success = false;

            if (entity != null)
            {
                entity.ObjectState = ObjectState.Deleted;
                this._dbSet.Attach(entity);

                success = true;
            }

            return success;
        }


        public void Update(TEntity entity)
        {
            entity.ObjectState = ObjectState.Modified;
            this._dbSet.Attach(entity);
            this._context.SyncObjectState(entity);
        }


        public virtual IQueryFluent<TEntity> Query()
        {
            return new QueryFluent<TEntity>(this);
        }

        public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return new QueryFluent<TEntity>(this, queryObject);
        }

        public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return new QueryFluent<TEntity>(this, query);
        }

        public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
        {
            return this._dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        public IQueryable<TEntity> Queryable()
        {
            return this._dbSet;
        }


        public IRepository<T> GetRepository<T>() where T : class, IObjectState
        {
            return this._unitOfWork.Repository<T>();
        }


        internal IQueryable<TEntity> Select(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = this._dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        internal async Task<IEnumerable<TEntity>> SelectAsync(
            Expression<Func<TEntity, bool>> query = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includes = null,
            int? page = null,
            int? pageSize = null)
        {
            //See: Best Practices in Asynchronous Programming http://msdn.microsoft.com/en-us/magazine/jj991977.aspx
            return await Task.Run(() => this.Select(query, orderBy, includes, page, pageSize).AsEnumerable()).ConfigureAwait(false);
        }


        private void SyncObjectGraph(object entity)
        {
            // Set tracking state for child collections
            foreach (var prop in entity.GetType().GetProperties())
            {
                // Apply changes to 1-1 and M-1 properties
                var trackableRef = prop.GetValue(entity, null) as IObjectState;
                if (trackableRef != null && trackableRef.ObjectState == ObjectState.Added)
                {
                    _dbSet.Attach((TEntity)entity);
                    _context.SyncObjectState((IObjectState)entity);
                }

                // Apply changes to 1-M properties
                var items = prop.GetValue(entity, null) as IList<IObjectState>;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        this.SyncObjectGraph(item);
                    }
                }
            }
        }
    }
}
