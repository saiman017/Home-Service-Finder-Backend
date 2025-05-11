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
       
        public static void AddPostgreSqlDateTruncSupport(this ModelBuilder modelBuilder)
        {
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
        public static DateTime DateTrunc(string field, DateTime date)
        {
            throw new NotSupportedException("This method is for use with Entity Framework Core only and cannot be used directly.");
        }
    }
}