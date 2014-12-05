using Entities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6.DataContext
{
    public abstract class FakeDbSet<TEntity> : DbSet<TEntity>, IDbSet<TEntity>
        where TEntity : Entity, new()
    {
        private readonly ObservableCollection<TEntity> _items;
        private readonly IQueryable _query;
        
        protected FakeDbSet()
        {
            this._items = new ObservableCollection<TEntity>();
            this._query = this._items.AsQueryable();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._items.GetEnumerator();
        }
        public IEnumerator<TEntity> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        public Expression Expression
        {
            get { return this._query.Expression; }
        }

        public Type ElementType
        {
            get { return this._query.ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return this._query.Provider; }
        }

        public override TEntity Add(TEntity entity)
        {
            this._items.Add(entity);
            return entity;
        }

        public override TEntity Remove(TEntity entity)
        {
            this._items.Remove(entity);
            return entity;
        }

        public override TEntity Attach(TEntity entity)
        {
            switch (entity.ObjectState)
            {
                case ObjectState.Unchanged:
                case ObjectState.Added:
                    this._items.Add(entity);
                    break;

                case ObjectState.Modified:
                    this._items.Remove(entity);
                    this._items.Add(entity);
                    break;

                case ObjectState.Deleted:
                    this._items.Remove(entity);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
                    break;
            }

            return entity;
        }

        public override TEntity Create()
        {
            return new TEntity();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<TEntity> Local
        {
            get
            {
                return this._items;
            }
        }
    }
}
