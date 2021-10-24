using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension.Internal;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension
{
    public static class AltairCaNpgsqlContextBuilderExtension
    {
        public static DbContextOptionsBuilder UseEncryptionFunctions(
            this DbContextOptionsBuilder optionsBuilder,string password,EncKeyLength encKeyLength)
        {
            var extension = (AltairCaNpgsqlContextOptionsExtension)GetOrCreateExtension(optionsBuilder,password,encKeyLength);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            return optionsBuilder;
        }

        private static AltairCaNpgsqlContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder,string password,EncKeyLength encKeyLength)
            => optionsBuilder.Options.FindExtension<AltairCaNpgsqlContextOptionsExtension>()
               ?? new AltairCaNpgsqlContextOptionsExtension(password,encKeyLength);
    }
}
