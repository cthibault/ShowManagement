using Entities.Pattern;
using Repository.Pattern.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Pattern
{
    public abstract class Service<TEntity> : IService<TEntity> where TEntity : class, IObjectState
    {
        private readonly IRepositoryAsync<TEntity> _repository;


        protected Service(IRepositoryAsync<TEntity> repository)
        {
            this._repository = repository;
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return this._repository.Find(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await this._repository.FindAsync(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await this._repository.FindAsync(cancellationToken, keyValues);
        }

        public virtual void Insert(TEntity entity)
        {
            this._repository.Insert(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            this._repository.InsertRange(entities);
        }

        public virtual void InsertOrUpdateGraph(TEntity entity)
        {
            this._repository.InsertOrUpdateGraph(entity);
        }

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            this._repository.InsertGraphRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            this._repository.Update(entity);
        }

        public virtual void Delete(object id)
        {
            this._repository.Delete(id);
        }

        public virtual void Delete(TEntity entity)
        {
            this._repository.Delete(entity);
        }

        public virtual async Task<bool> DeleteAsync(params object[] keyValues)
        {
            return await this._repository.DeleteAsync(keyValues);
        }

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await this._repository.DeleteAsync(cancellationToken, keyValues);
        }

        public IQueryFluent<TEntity> Query()
        {
            return this._repository.Query();
        }

        public virtual IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject)
        {
            return this._repository.Query(queryObject);
        }

        public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
        {
            return this._repository.SelectQuery(query, parameters);
        }

        public virtual IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return this._repository.Query(query);
        }

        public IQueryable<TEntity> Queryable()
        {
            return this._repository.Queryable();
        }
    }
}
