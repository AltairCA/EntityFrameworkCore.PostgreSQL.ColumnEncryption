using System.Collections.Generic;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension.Internal
{
    internal class AltairCaNpgsqlContextOptionsExtension:IDbContextOptionsExtension
    {
        
        public AltairCaNpgsqlContextOptionsExtension(string password,EncKeyLength encKeyLength)
        {
            int keyLength = AesUtil.GetKeyLength(encKeyLength);
            AltairCaFunctionImplementation.Password = AesUtil.PasswordFixer(password,keyLength);
            AltairCaFunctionImplementation.Iv = AesUtil.IvFixer(password,keyLength);
        }
        private DbContextOptionsExtensionInfo _info;
        public void ApplyServices(IServiceCollection services)
        {
            new EntityFrameworkRelationalServicesBuilder(services).TryAdd<IMethodCallTranslatorPlugin,AltairCaNpgsqlMethodCallTranslatorPlugin>();
        }

        public void Validate(IDbContextOptions options)
        {
            
        }

        public DbContextOptionsExtensionInfo Info => this._info ?? (MyDbContextOptionsExtensionInfo)new MyDbContextOptionsExtensionInfo((IDbContextOptionsExtension)this);

        private sealed class MyDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
        {
            public MyDbContextOptionsExtensionInfo(IDbContextOptionsExtension instance) : base(instance) { }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment => "";

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return true;
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
