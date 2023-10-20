using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension;

public static class PropertyExtension
{
    public static PropertyBuilder<TProperty> UseEncryption<TProperty>
        (this PropertyBuilder<TProperty> builder, string password, EncKeyLength encKeyLength)
    {
        var keyLength = AesUtil.GetKeyLength(encKeyLength);
        var fixedPassword = AesUtil.PasswordFixer(password, keyLength);
        var iv = AesUtil.IvFixer(password, keyLength);
        builder.HasConversion(new EncryptionValueConverter(keyLength, fixedPassword, iv));
        return builder;
    }
}