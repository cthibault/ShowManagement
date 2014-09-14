using ShowManagement.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ShowManagement.Web.Data.Repositories
{
    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetById(object id);

        void Insert(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entity);

        void Update(TEntity entity);
    }

    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        internal ShowManagementDbContext Context;
        internal DbSet<TEntity> DbSet;

        public BaseRepository(ShowManagementDbContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "")
        {
            IQueryable<TEntity> query = this.DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            List<TEntity> result = null;

            if (orderBy != null)
            {
                result = orderBy(query).ToList();
            }
            else
            {
                result = query.ToList();
            }

            return result;
        }

        public TEntity GetById(object id)
        {
            TEntity entity = null;

            if (id != null)
            {
                entity = this.DbSet.Find(id);
            }

            return entity;
        }

        public void Insert(TEntity entity)
        {
            if (entity != null)
            {
                this.DbSet.Add(entity);
            }
        }

        public void Delete(object id)
        {
            TEntity entity = null;

            if (id != null)
            {
                entity = this.DbSet.Find(id);

                this.Delete(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            if (entity != null)
            {
                if (this.Context.Entry(entity).State == EntityState.Detached)
                {
                    this.DbSet.Attach(entity);
                }

                this.DbSet.Remove(entity);
            }
        }

        public void Update(TEntity entity)
        {
            if (entity != null)
            {
                this.DbSet.Attach(entity);
                this.Context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}