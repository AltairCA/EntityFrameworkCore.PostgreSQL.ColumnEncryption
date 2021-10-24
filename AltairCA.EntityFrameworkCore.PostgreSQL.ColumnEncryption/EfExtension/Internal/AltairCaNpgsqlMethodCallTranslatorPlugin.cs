using System.Collections.Generic;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension.Internal
{
    internal class AltairCaNpgsqlMethodCallTranslatorPlugin:NpgsqlMethodCallTranslatorProvider
    {
        public AltairCaNpgsqlMethodCallTranslatorPlugin(RelationalMethodCallTranslatorProviderDependencies dependencies, IRelationalTypeMappingSource typeMappingSource) : base(dependencies, typeMappingSource)
        {
            ISqlExpressionFactory expressionFactory = dependencies.SqlExpressionFactory;
            this.AddTranslators(new List<IMethodCallTranslator>
            {
                new AltairCaFunctionImplementation(expressionFactory)
            });
        }
    }
}
