using System.Linq;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Attribute;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;
using Microsoft.EntityFrameworkCore;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension
{
    public static class AttributeExtension
    {
        public static ModelBuilder UseEncryptAttribute(this ModelBuilder builder, string password, EncKeyLength encKeyLength)
        {
            var keyLength = AesUtil.GetKeyLength(encKeyLength);
            var fixedPassword = AesUtil.PasswordFixer(password, keyLength);
            var iv = AesUtil.IvFixer(password, keyLength);
            
            
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties().Where(p => p.PropertyInfo != null))
                {
                    var attributes = property.PropertyInfo.GetCustomAttributes(typeof(NpgsqlEncryptAttribute), false);
                    if (attributes.Any())
                    {
                        property.SetValueConverter(new EncryptionValueConverter(keyLength, fixedPassword, iv));
                    }
                }
            }
            return builder;
        }
    }
}
