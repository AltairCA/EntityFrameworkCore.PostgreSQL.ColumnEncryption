using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension
{
    public static class AltairCaNpgsqlContextBuilderExtension
    {
        public static DbContextOptionsBuilder UseEncryptionFunctions(
            this DbContextOptionsBuilder optionsBuilder,string password)
        {
            var extension = (AltairCaNpgsqlContextOptionsExtension)GetOrCreateExtension(optionsBuilder,password);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            return optionsBuilder;
        }

        private static AltairCaNpgsqlContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder,string password)
            => optionsBuilder.Options.FindExtension<AltairCaNpgsqlContextOptionsExtension>()
               ?? new AltairCaNpgsqlContextOptionsExtension(password);
    }
}
