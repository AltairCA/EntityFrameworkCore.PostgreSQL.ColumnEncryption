

using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Attribute
{
    internal static class AttributeEncryptExtension
    {
        public static string NpgsqlEncrypt(this string value, string password,string iv,int keyLength)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return AesUtil.AES_encrypt(value, password,iv,keyLength);
        }
        public static string NpgsqlDecrypt(this string value, string password,string iv,int keyLength)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            return AesUtil.AES_decrypt(value, password,iv,keyLength);
        }
    }
}