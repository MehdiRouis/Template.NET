using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using TemplateBase.Data.GlobalFilters;

namespace TemplateBase.Data
{
    public class ApplicationDbContext : DbContext, IDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //COMMON
        //public DbSet<Civility> Civilities { get; set; }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The DbDataReader object that lets us read the return data.</returns>
        public DataSet ExecuteSqlCommandWithReturn(string sql, params NpgsqlParameter[] parameters)
        {
            var result = Database.ExecuteSqlRaw(sql, parameters);

            var connection = (NpgsqlConnection)Database.GetDbConnection();

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);

            var dSet = new DataSet();
            var da = new NpgsqlDataAdapter(command);

            da.Fill(dSet);

            connection.Close();

            return dSet;
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues["UpdatedAtOnUtc"] = now;
                        break;

                    case EntityState.Added:
                        entry.CurrentValues["CreatedAtUtc"] = now;
                        entry.CurrentValues["UpdatedAtOnUtc"] = now;
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        if ((bool)entry.CurrentValues["ForceDelete"] != true)
                        {
                            entry.CurrentValues["UpdatedAtOnUtc"] = now;
                            entry.CurrentValues["IsDeleted"] = true;
                            entry.State = EntityState.Modified;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    // Vérifie si la propriété est de type DateTime (ou DateTime nullable)
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        // Applique un ValueConverter pour forcer l'UTC
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),  // Convertit en UTC avant la sauvegarde
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)   // Convertit en UTC lors de la récupération
                        ));
                    }
                }
            }
            base.OnModelCreating(builder);

            #region Seeds
            // Here...
            #endregion Seeds

            #region Filters
            builder.ApplyGlobalFilter(e => !e.IsDeleted);
            #endregion Filters
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="doNotEnsureTransaction">false - the transaction creation is not ensured; true - the transaction creation is ensured.</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            var result = Database.ExecuteSqlRaw(sql, parameters);

            return result;
        }
    }
}
