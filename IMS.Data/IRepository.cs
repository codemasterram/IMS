﻿using IMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Data
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T> where T : class
    {
        /// <summary>
        /// Returns the queryable entity set for the given type {T}.
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Returns an untracked queryable entity set for the given type {T}.
        /// The entities returned will not be cached in the object context thus increasing performance.
        /// </summary>
        IQueryable<T> TableUntracked { get; }

        /// <summary>
        /// Provides access to the entities currently being tracked by the context and have not been marked as deleted
        /// </summary>
        ICollection<T> Local { get; }

        /// <summary>
        /// Creates a new instance of an entity of type {T}
        /// </summary>
        /// <returns>The new entity instance.</returns>
        T Create();

        void Create(T entity);
        /// <summary>
        /// Gets an entity by id from the database or the local change tracker.
        /// </summary>
        /// <param name="id">The id of the entity. This can also be a composite key.</param>
        /// <returns>The resolved entity</returns>
        T GetById(object id);

        /// <summary>
        /// Attaches an entity to the context
        /// </summary>
        /// <param name="entity">The entity to attach</param>
        /// <returns>The entity</returns>
        T Attach(T entity);

        /// <summary>
        /// Marks the entity instance to be saved to the store.
        /// </summary>
        /// <param name="entity">An entity instance that should be saved to the database.</param>
        /// <remarks>Implementors should delegate this to the current <see cref="IDbContext" /></remarks>
        void Insert(T entity);

        /// <summary>
        /// Marks multiple entities to be saved to the store.
        /// </summary>
        /// <param name="entities">The list of entity instances to be saved to the database</param>
        /// <param name="batchSize">The number of entities to insert before saving to the database (if <see cref="AutoCommitEnabled"/> is true)</param>
        void InsertRange(IEnumerable<T> entities, int batchSize = 100);

        /// <summary>
        /// Marks the entity instance to be saved
        /// </summary>
        /// <param name="entity">An entity instance that should be saved to the database.</param>
        void MarkForInsert(T entity);

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance that should be updated in the database.</param>
        /// <remarks>Implementors should delegate this to the current <see cref="IDbContext" /></remarks>
        void Update(T entity);

        /// <summary>
        /// Marks the changes of existing entities to be saved to the store.
        /// </summary>
        /// <param name="entities">A list of entity instances that should be updated in the database.</param>
        /// <remarks>Implementors should delegate this to the current <see cref="IDbContext" /></remarks>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Marks the changes of an existing entity to be saved.
        /// </summary>
        /// <param name="entity">An instance that should be updated in the database.</param>
        void MarkForUpdate(T entity);

        /// <summary>
        /// Marks an existing entity to be deleted from the store.
        /// </summary>
        /// <param name="entity">An entity instance that should be deleted from the database.</param>
        /// <remarks>Implementors should delegate this to the current <see cref="IDbContext" /></remarks>
        void Delete(T entity);

        /// <summary>
        /// Marks an existing entity to be deleted by id.
        /// </summary>
        /// <param name="id">Entity ID</param>
        void Delete(int id);
        /// <summary>
        /// if not successed to delete returns the error message.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<string> TryDelete(int id, out string message);

        /// <summary>
        /// Marks existing entities to be deleted from the store.
        /// </summary>
        /// <param name="entities">A list of entity instances that should be deleted from the database.</param>
        /// <remarks>Implementors should delegate this to the current <see cref="IDbContext" /></remarks>
        void DeleteRange(IEnumerable<T> entities);

        /// <summary>
        /// Gets a value indicating whether the given entity was modified since it has been attached to the context
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <returns><c>true</c> if the entity was modified, <c>false</c> otherwise</returns>
        bool IsModified(T entity);

        /// <summary>
        /// Commit Changes
        /// </summary>
        void CommitChanges();

        /// <summary>
        /// Returns the data context associated with the repository.
        /// </summary>
        /// <remarks>
        /// The context is likely shared among multiple repository types.
        /// So committing data or changing configuration also affects other repositories. 
        /// </remarks>
        IDbContext Context { get; }

        /// <summary>
        /// Gets or sets a value indicating whether database write operations
        /// such as insert, delete or update should be committed immediately.
        /// </summary>
		/// <remarks>
		/// Set this to <c>true</c> or <c>false</c> to supersede the global <c>AutoCommitEnabled</c>
		/// on <see cref="IDbContext"/> level for this repository instance only.
		/// </remarks>
        bool? AutoCommitEnabled { get; set; }
    }
}
