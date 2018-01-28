using IMS.Data.Extensions;
using IMS.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace IMS.Data
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly IDbContext _context;
        private IDbSet<T> _entities;

        public EfRepository(IDbContext context)
        {
            this._context = context;
            AutoCommitEnabled = true;
        }

        #region interface members

        public virtual IQueryable<T> Table
        {
            get
            {
                if (_context.ForceNoTracking)
                {
                    return this.Entities.AsNoTracking();
                }
                return this.Entities;
            }
        }

        public virtual IQueryable<T> TableUntracked
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }

        public virtual ICollection<T> Local
        {
            get
            {
                return this.Entities.Local;
            }
        }

        public virtual T Create()
        {
            return this.Entities.Create();
        }

        public virtual T GetById(object id)
        {
            return this.Entities.Find(id);
        }

        public virtual T Attach(T entity)
        {
            return this.Entities.Attach(entity);
        }

        public virtual void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.Entities.Add(entity);

            if (this.AutoCommitEnabledInternal)
                _context.SaveChanges();
        }

        public virtual void Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.Entities.Add(entity);

            if (this.AutoCommitEnabledInternal)
                _context.SaveChanges();
        }

        public virtual void InsertRange(IEnumerable<T> entities, int batchSize = 100)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                if (entities.Any())
                {
                    if (batchSize <= 0)
                    {
                        // insert all in one step
                        entities.Each(x => this.Entities.Add(x));
                        if (this.AutoCommitEnabledInternal)
                            _context.SaveChanges();
                    }
                    else
                    {
                        int i = 1;
                        bool saved = false;
                        foreach (var entity in entities)
                        {
                            this.Entities.Add(entity);
                            saved = false;
                            if (i % batchSize == 0)
                            {
                                if (this.AutoCommitEnabledInternal)
                                    _context.SaveChanges();
                                i = 0;
                                saved = true;
                            }
                            i++;
                        }

                        if (!saved)
                        {
                            if (this.AutoCommitEnabledInternal)
                                _context.SaveChanges();
                        }
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            SetEntityStateToModifiedIfApplicable(entity);

            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            entities.Each(entity =>
            {
                SetEntityStateToModifiedIfApplicable(entity);
            });

            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        private void SetEntityStateToModifiedIfApplicable(T entity)
        {
            var entry = InternalContext.Entry(entity);
            if (entry.State < System.Data.Entity.EntityState.Added || (this.AutoCommitEnabledInternal && !InternalContext.Configuration.AutoDetectChangesEnabled))
            {
                entry.State = System.Data.Entity.EntityState.Modified;
            }
        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (InternalContext.Entry(entity).State == System.Data.Entity.EntityState.Detached)
            {
                this.Entities.Attach(entity);
            }

            this.Entities.Remove(entity);

            if (this.AutoCommitEnabledInternal)
                _context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
                Delete(entity);
        }

        private string GetExceptionMessages(Exception ex)
        {
            if (ex == null) return string.Empty;
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                msg = GetExceptionMessages(ex.InnerException);
            }
            return msg;
        }

        public List<string> TryDelete(int id, out string errorMessage)
        {
            errorMessage = null;
            var errors = new List<string>();
            try
            {
                var entity = GetById(id);
                if (entity != null)
                    Delete(entity);
            }
            catch (Exception ex)
            {
                if (GetExceptionMessages(ex).Contains("REFERENCE"))
                {
                    errorMessage = "This item is used in another module. Unable to delete this item.";
                }

                errors.Add(ex.Message);
            }
            return errors;
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            entities.Each(entity =>
            {
                if (InternalContext.Entry(entity).State == System.Data.Entity.EntityState.Detached)
                {
                    this.Entities.Attach(entity);
                }
            });

            this.Entities.RemoveRange(entities);

            if (this.AutoCommitEnabledInternal)
                _context.SaveChanges();
        }

        public virtual bool IsModified(T entity)
        {
            var ctx = InternalContext;
            var entry = ctx.Entry(entity);

            if (entry != null)
            {
                var modified = entry.State == System.Data.Entity.EntityState.Modified;
                return modified;
            }

            return false;
        }

        public void MarkForInsert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            this.Entities.Add(entity);
        }

        public void MarkForUpdate(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            SetEntityStateToModifiedIfApplicable(entity);
        }

        public void CommitChanges()
        {
            _context.SaveChanges();
        }

        public virtual IDbContext Context
        {
            get { return _context; }
        }

        public bool? AutoCommitEnabled { get; set; }

        private bool AutoCommitEnabledInternal
        {
            get
            {
                return this.AutoCommitEnabled ?? _context.AutoCommitEnabled;
            }
        }

        #endregion

        #region Helpers

        protected internal IMSDbContext InternalContext
        {
            get { return _context as IMSDbContext; }
        }

        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities as DbSet<T>;
            }
        }

        #endregion

    }
}