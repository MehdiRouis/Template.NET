using Template.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Template.Data.GlobalFilters
{
    public static class ApplyFilter
    {
        public static void ApplyGlobalFilter(this ModelBuilder builder, Expression<Func<BaseEntity, bool>> expression)
        {
            var entities = builder.Model.GetEntityTypes().Select(e => e.ClrType);
            foreach (var entity in entities)
            {
                var newParam = Expression.Parameter(entity);
                var newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                builder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
            }
        }
    }
}