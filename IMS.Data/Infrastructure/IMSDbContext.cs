using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Data.Infrastructure
{
    /// <summary>
    /// IMSDbContext
    /// </summary>
    public partial class IMSDbContext : DbContext, IDbContext
    {
        public string CurrentUserName { get; set; }

        private List<string> logSkippedTables
        {
            get
            {
                return new List<string>
                {
                    "TransactionLog", "LoginHistory","PasswordResetLog"
                };
            }
        }

        #region IDbContext members

        public virtual string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        private IEnumerable<DbParameter> ToParameters(params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return Enumerable.Empty<DbParameter>();

            return parameters.Cast<DbParameter>();
        }

        public virtual IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : class, new()
        {
            // Add parameters to command
            var commandText2 = commandText;
            var dbParams = ToParameters(parameters);
            bool firstParam = true;
            foreach (var p in dbParams)
            {
                commandText += firstParam ? " " : ", ";
                firstParam = false;

                commandText += "@" + p.ParameterName;
                if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                {
                    // output parameter
                    commandText += " output";
                }
            }

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
            return result;
        }

        private IList<TEntity> ExecuteStoredProcedureListLegacy<TEntity>(string commandText, IEnumerable<DbParameter> parameters) where TEntity : class, new()
        {
            var connection = this.Database.Connection;
            // Don't close the connection after command execution

            // open the connection for use
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            // create a command object
            using (var cmd = connection.CreateCommand())
            {
                // command to execute
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;

                // move parameters to command object
                cmd.Parameters.AddRange(parameters.ToArray());

                // database call
                var reader = cmd.ExecuteReader();
                var result = ((IObjectContextAdapter)(this)).ObjectContext.Translate<TEntity>(reader).ToList();
                if (!ForceNoTracking)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        result[i] = Attach(result[i]);
                    }
                }
                // close up the reader, we're done saving results
                reader.Close();
                return result;
            }
        }

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  
        /// The type can be any type that has properties that match the names of the columns returned from the query, 
        /// or can be a simple primitive type. The type does not have to be an entity type. 
        /// The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
            }

            var transactionalBehavior = doNotEnsureTransaction
                ? TransactionalBehavior.DoNotEnsureTransaction
                : TransactionalBehavior.EnsureTransaction;
            var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
            }

            return result;
        }

        public bool HasChanges
        {
            get
            {
                return this.ChangeTracker.Entries()
                           .Where(x => x.State != System.Data.Entity.EntityState.Unchanged && x.State != System.Data.Entity.EntityState.Detached)
                           .Any();
            }
        }

        public bool OverrideLogSave { get; set; }
        public override int SaveChanges()
        {
            //audit log here
            var addedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added).Select(x => new AuditLog { Entity = x.Entity }).ToList();
            var modifiedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).Select(x => new AuditLog { Entity = UnProxy(this, x.Entity), Entry = x.OriginalValues }).ToList();
            var deletedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted).Select(x => new AuditLog { TableName = x.Entity.GetType().BaseType.Name, Entry = x.OriginalValues }).ToList();

            // SAVE NOW!!!
            bool validateOnSaveEnabled = this.Configuration.ValidateOnSaveEnabled;
            this.Configuration.ValidateOnSaveEnabled = false;
            int result = base.SaveChanges();

            foreach (var entity in addedEntities)
            {
                this.Entry(entity.Entity).GetDatabaseValues();
            }


            this.Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled;
            return result;
        }

        public override Task<int> SaveChangesAsync()
        {
            // SAVE NOW!!!
            bool validateOnSaveEnabled = this.Configuration.ValidateOnSaveEnabled;
            this.Configuration.ValidateOnSaveEnabled = false;
            var result = base.SaveChangesAsync();

            result.ContinueWith((t) =>
            {
                this.Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled;
            });

            return result;
        }

        // required for UoW implementation
        public string Alias { get; internal set; }

        // performance on bulk inserts
        public bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }

        // performance on bulk inserts
        public bool ValidateOnSaveEnabled
        {
            get
            {
                return this.Configuration.ValidateOnSaveEnabled;
            }
            set
            {
                this.Configuration.ValidateOnSaveEnabled = value;
            }
        }

        public bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        public bool LazyLoadingEnabled
        {
            get
            {
                return this.Configuration.LazyLoadingEnabled;
            }
            set
            {
                this.Configuration.LazyLoadingEnabled = value;
            }
        }

        public bool ForceNoTracking { get; set; }

        public bool AutoCommitEnabled { get; set; }

        public DbContextTransactionWrapper BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            var dbContextTransaction = this.Database.BeginTransaction(isolationLevel);
            return new DbContextTransactionWrapper(dbContextTransaction);
        }

        public void UseTransaction(DbTransaction transaction)
        {
            this.Database.UseTransaction(transaction);
        }

        #endregion

        #region Utils

        /// <summary>
        /// Resolves the connection string from the <c>Settings.txt</c> file
        /// </summary>
        /// <returns>The connection string</returns>
        /// <remarks>This helper is called from parameterless DbContext constructors which are required for EF tooling support.</remarks>
        public static string GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["NTADBConnection"].ConnectionString;
            return connectionString;
        }

        public TEntity Attach<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = Set<TEntity>();

            ObjectStateEntry entry;
            var adapter = (IObjectContextAdapter)this;

            if (adapter.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                if (entry.State != EntityState.Detached)
                    return (TEntity)entry.Entity;
                else
                    dbSet.Attach(entity);
            }
            return entity;
        }

        public bool IsAttached<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity != null)
            {
                ObjectStateEntry entry;
                var adapter = (IObjectContextAdapter)this;

                if (adapter.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
                {
                    return entry.State != EntityState.Detached;
                }

                return false;
            }

            return false;
        }

        public void DetachEntity<TEntity>(TEntity entity) where TEntity : class
        {
            this.Entry(entity).State = System.Data.Entity.EntityState.Detached;
        }

        public int DetachEntities<TEntity>(bool unchangedEntitiesOnly = true) where TEntity : class
        {
            Func<DbEntityEntry, bool> predicate = x =>
            {
                if (x.Entity is TEntity)
                {
                    if (x.State == System.Data.Entity.EntityState.Detached)
                        return false;

                    if (unchangedEntitiesOnly)
                        return x.State == System.Data.Entity.EntityState.Unchanged;

                    return true;
                }

                return false;
            };

            var attachedEntities = this.ChangeTracker.Entries().Where(predicate).ToList();
            attachedEntities.ForEach(entry => entry.State = System.Data.Entity.EntityState.Detached);
            return attachedEntities.Count;
        }

        public void ChangeState<TEntity>(TEntity entity, System.Data.Entity.EntityState newState) where TEntity : class
        {
            Console.WriteLine("ChangeState ORIGINAL");
            this.Entry(entity).State = newState;
        }

        public void ReloadEntity<TEntity>(TEntity entity) where TEntity : class
        {
            this.Entry(entity).Reload();
        }

        private object UnProxy(DbContext context, object proxyObject)
        {
            var proxyCreationEnabled = context.Configuration.ProxyCreationEnabled;
            try
            {
                context.Configuration.ProxyCreationEnabled = false;
                var poco = context.Entry(proxyObject).CurrentValues.ToObject();
                return poco;
            }
            finally
            {
                context.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
            }
        }

        #endregion
    }

    public class AuditLog
    {
        public Dictionary<string, string> OriginalValues { get; set; }

        private DbPropertyValues _entry;
        public DbPropertyValues Entry
        {
            get
            {
                return _entry;
            }
            set
            {
                _entry = value;
                foreach (var p in value.PropertyNames)
                {
                    OriginalValues.Add(p, value[p] != null ? value[p].ToString() : null);
                }
            }
        }
        public string TableName { get; set; }
        public object Entity { get; set; }

        public AuditLog()
        {
            OriginalValues = new Dictionary<string, string>();
        }
    }
}