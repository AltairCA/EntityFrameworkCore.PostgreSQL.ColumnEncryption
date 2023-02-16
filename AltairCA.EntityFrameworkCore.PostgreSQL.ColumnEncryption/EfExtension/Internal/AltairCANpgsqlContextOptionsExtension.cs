using System.Collections.Generic;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension.Internal
{
    internal class AltairCaNpgsqlContextOptionsExtension : IDbContextOptionsExtension
    {

        private readonly string _password;
        private readonly string _iv;

        public AltairCaNpgsqlContextOptionsExtension(string password, EncKeyLength encKeyLength)
        {
            int keyLength = AesUtil.GetKeyLength(encKeyLength);
            _password = AesUtil.PasswordFixer(password, keyLength);
            _iv = AesUtil.IvFixer(password, keyLength);
        }
        private DbContextOptionsExtensionInfo _info;
        public void ApplyServices(IServiceCollection services)
        {
            new EntityFrameworkRelationalServicesBuilder(services)
                .TryAdd<IMethodCallTranslatorPlugin, AltairCaNpgsqlMethodCallTranslatorPlugin>(
                    e => new AltairCaNpgsqlMethodCallTranslatorPlugin(
                        e.GetRequiredService<IRelationalTypeMappingSource>(), e.GetRequiredService<ISqlExpressionFactory>(),
                        _password, _iv)
                );
        }

        public void Validate(IDbContextOptions options)
        {

        }

        public DbContextOptionsExtensionInfo Info => _info ?? new MyDbContextOptionsExtensionInfo((IDbContextOptionsExtension)this);

        private sealed class MyDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public MyDbContextOptionsExtensionInfo(IDbContextOptionsExtension instance) : base(instance) { }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => "";

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return false;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {

            }

            public override int GetServiceProviderHashCode()
            {
                return 0;
            }
        }
    }
}
