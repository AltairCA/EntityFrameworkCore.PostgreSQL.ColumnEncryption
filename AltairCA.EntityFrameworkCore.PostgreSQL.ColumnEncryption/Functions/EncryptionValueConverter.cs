using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Attribute;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;

internal class EncryptionValueConverter:ValueConverter<string,string>
{
    public EncryptionValueConverter(
        int keyLength,
        string password,
        string iv) 
        : base(
            rawString => rawString.NpgsqlEncrypt(password, iv, keyLength), 
            encryptedString => encryptedString.NpgsqlDecrypt(password, iv, keyLength))
    {
    }
}