using Entities.Pattern;
using Repository.Pattern.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6.DataContext
{
    public interface IFakeDbContext : IDataContextAsync
    {
        DbSet<T> Set<T>() where T : class;

        void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : Entity, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new();
    }
}
