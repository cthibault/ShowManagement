using Entities.Pattern;
using Repository.Pattern.DataContext;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Pattern.Ef6.Repository
{
    /// <summary>
    ///     A maker of Repositories.
    /// </summary>
    /// <remarks>
    ///     An instance of this class contains repository factory functions for different types.
    ///     Each factory function takes an EF <see cref="IDataContextAsync" /> and returns
    ///     a repository bound to that DataContext.
    ///     <para>
    ///         Designed to be a "Singleton", configured at web application start with
    ///         all of the factory functions needed to create any type of repository.
    ///         Should be thread-safe to use because it is configured at app start,
    ///         before any request for a factory, and should be immutable thereafter.
    ///     </para>
    /// </remarks>
    public class RepositoryFactories
    {
        /// <summary>
        ///     Get the dictionary of repository factory functions.
        /// </summary>
        /// <remarks>
        ///     A dictionary key is a System.Type, typically a repository type.
        ///     A value is a repository factory function
        ///     that takes a <see cref="DbContext" /> argument and returns
        ///     a repository object. Caller must know how to cast it.
        /// </remarks>
        private readonly IDictionary<Type, Func<IDataContextAsync, IUnitOfWorkAsync, dynamic>> _repositoryFactories;

        /// <summary>
        ///     Constructor that initializes with runtime repository factories
        /// </summary>
        public RepositoryFactories()
        {
            this._repositoryFactories = this.GetFactories();
        }
        public RepositoryFactories(Dictionary<Type, Func<dynamic>> customRepositoryFactories)
        {
            _repositoryFactories = GetFactories();
        }


        /// <summary>
        ///     Get the repository factory function for the type.
        /// </summary>
        /// <typeparam name="T">Type serving as the repository factory lookup key.</typeparam>
        /// <returns>The repository function if found, else null.</returns>
        /// <remarks>
        ///     The type parameter, T, is typically the repository type
        ///     but could be any type (e.g., an entity type)
        /// </remarks>
        public Func<IDataContextAsync, IUnitOfWorkAsync, dynamic> GetRepositoryFactory<T>()
        {
            Func<IDataContextAsync, IUnitOfWorkAsync, dynamic> factory = null;

            this._repositoryFactories.TryGetValue(typeof(T), out factory);
            
            return factory;
        }

        /// <summary>
        ///     Get the factory for <see cref="IRepository{TEntity}" /> where T is a reference type and implements IObjectState.
        /// </summary>
        /// <typeparam name="T">The root type of the repository, typically an entity type.</typeparam>
        /// <returns>
        ///     A factory that creates the <see cref="IRepository{T}" />, given an EF <see cref="DbContext" />.
        /// </returns>
        /// <remarks>
        ///     Looks first for a custom factory in <see cref="_repositoryFactories" />.
        ///     If not, falls back to the <see cref="DefaultEntityRepositoryFactory{T}" />.
        ///     You can substitute an alternative factory for the default one by adding
        ///     a repository factory for type "T" to <see cref="_repositoryFactories" />.
        /// </remarks>
        public Func<IDataContextAsync, IUnitOfWorkAsync, dynamic> GetRepositoryFactoryForEntityType<TEntity>() where TEntity : class, IObjectState
        {
            return this.GetRepositoryFactory<TEntity>() ?? this.DefaultEntityRepositoryFactory<TEntity>();
        }


        protected virtual Func<IDataContextAsync, IUnitOfWorkAsync, dynamic> DefaultEntityRepositoryFactory<TEntity>() where TEntity : class, IObjectState
        {
            return (dbContext, uow) => new Repository<TEntity>(dbContext, uow);
        }


        /// <summary>
        ///     Return the runtime repository factory functions,
        ///     each one is a factory for a repository of a particular type.
        /// </summary>
        /// <remarks>
        ///     MODIFY THIS METHOD TO ADD CUSTOM FACTORY (FUNCTIONS)
        /// </remarks>
        private IDictionary<Type, Func<IDataContextAsync, IUnitOfWorkAsync, dynamic>> GetFactories()
        {
            return new Dictionary<Type, Func<IDataContextAsync, IUnitOfWorkAsync, dynamic>>();
        }
    }
}
