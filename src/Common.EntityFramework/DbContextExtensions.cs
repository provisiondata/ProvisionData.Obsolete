using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ProvisionData.EntityFramework
{
    public static class DbContextExtensions
    {
        public static void LoadRelated<TEntity, TReference>(this DbContext context, TEntity entity, Expression<Func<TEntity, TReference>> selector)
            where TEntity : class
            where TReference : class
        {
            var reference = context.Entry(entity).Reference(selector);
            if (!reference.IsLoaded)
                reference.Load();
        }

        public static void LoadRelated<TEntity, TReference>(this DbContext context, TEntity entity, Expression<Func<TEntity, IEnumerable<TReference>>> selector)
            where TEntity : class
            where TReference : class
        {
            var collection = context.Entry(entity).Collection(selector);
            if (!collection.IsLoaded)
                collection.Load();
        }
    }
}
