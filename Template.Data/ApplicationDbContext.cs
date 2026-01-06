using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using System.Data;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Entities.Users;
using Template.Data.GlobalFilters;

namespace Template.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //USERS
        public DbSet<User> Users { get; set; }
        public DbSet<UserPassword> UsersPasswords { get; set; }
        public DbSet<UserSession> UsersSessions { get; set; }


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
            var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity);
            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues["UpdatedAt"] = now;
                        break;

                    case EntityState.Added:
                        entry.CurrentValues["CreatedAt"] = now;
                        entry.CurrentValues["UpdatedAt"] = now;
                        break;
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        if ((bool?)entry.CurrentValues["ForceDelete"] != true)
                        {
                            entry.CurrentValues["UpdatedAt"] = now;
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
                    var clrType = property.ClrType;

                    // ===== Traitement DateTime / DateTime? pour forcer l'UTC =====
                    if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }

                    // ===== Traitement Enum / Enum nullable pour générer les commentaires =====
                    Type enumType = Nullable.GetUnderlyingType(clrType) ?? clrType;
                    if (enumType.IsEnum)
                    {
                        var values = Enum.GetValues(enumType).Cast<object>().Select(v => $"{v} = {(int)v}");
                        var comment = string.Join(Environment.NewLine, values);
                        property.SetComment(comment);
                    }
                }
            }
            base.OnModelCreating(builder);

            #region Seeds
            #endregion Seeds

            #region Filters
            builder.ApplyGlobalFilter(e => !e.IsDeleted);
            #endregion Filters

            #region Relations

            #endregion Relations
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
            using var transaction = Database.BeginTransaction();
            try
            {
                var result = Database.ExecuteSqlRaw(sql, parameters);
                transaction.Commit();
                return result;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
