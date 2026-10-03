using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Baitul_Kitab.Models.Helper
{
    using System.Linq.Expressions;

    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder(source, propertyName, "OrderBy");
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return ApplyOrder(source, propertyName, "OrderByDescending");
        }

        private static IQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "x");

            Expression property = propertyName.Split('.')
                .Aggregate<string, Expression>(parameter, Expression.Property);

            var selector = Expression.Lambda(property, parameter);

            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(type, property.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, selector });
        }
    }


}
