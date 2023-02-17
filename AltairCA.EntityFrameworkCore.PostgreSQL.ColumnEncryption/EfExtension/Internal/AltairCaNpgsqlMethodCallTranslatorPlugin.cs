using System;
using System.Collections.Generic;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension.Internal
{
    internal class AltairCaNpgsqlMethodCallTranslatorPlugin : IMethodCallTranslatorPlugin
    {
        public AltairCaNpgsqlMethodCallTranslatorPlugin(IRelationalTypeMappingSource typeMappingSource,
            ISqlExpressionFactory sqlExpressionFactory, string password, string iv)

        {
            if (!(sqlExpressionFactory is NpgsqlSqlExpressionFactory npgsqlSqlExpressionFactory))
            {
                throw new ArgumentException($"Must be an {nameof(NpgsqlSqlExpressionFactory)}", nameof(sqlExpressionFactory));
            }
            Translators = new IMethodCallTranslator[] { new AltairCaFunctionImplementation(sqlExpressionFactory, password, iv), };
        }

        public IEnumerable<IMethodCallTranslator> Translators { get; }
    }
}
