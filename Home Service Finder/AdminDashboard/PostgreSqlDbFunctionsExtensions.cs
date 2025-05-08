using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using System.Reflection;

namespace Home_Service_Finder.Extensions
{
    public static class PostgreSqlDbFunctionsExtensions
    {
        /// <summary>
        /// Registers PostgreSQL date_trunc function with EF Core
        /// </summary>
        public static void AddPostgreSqlDateTruncSupport(this ModelBuilder modelBuilder)
        {
            // Register the function with EF Core
            modelBuilder.HasDbFunction(typeof(PostgreSqlDbFunctionsExtensions)
                .GetMethod(nameof(DateTrunc), new[] { typeof(string), typeof(DateTime) }))
                .HasTranslation(args =>
                {
                    var sqlFunctionExpression = new SqlFunctionExpression(
                        "date_trunc",
                        args,
                        true,
                        new[] { true, true },
                        typeof(DateTime),
                        null);
                    return sqlFunctionExpression;
                });
        }

        /// <summary>
        /// PostgreSQL date_trunc function
        /// </summary>
        /// <param name="field">Field to truncate to ('day', 'week', 'month', 'year', etc.)</param>
        /// <param name="date">Date to truncate</param>
        /// <returns>Truncated date</returns>
        public static DateTime DateTrunc(string field, DateTime date)
        {
            throw new NotSupportedException("This method is for use with Entity Framework Core only and cannot be used directly.");
        }
    }
}