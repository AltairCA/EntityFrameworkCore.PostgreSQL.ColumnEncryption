using System.Linq;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Attribute;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension
{
    public static class AttributeExtension
    {
		private static readonly ValueConverter<string, string> _converter
	 = new ValueConverter<string, string>(color => color.NpgsqlEncrypt(Password,Iv,KeyLength),
										 name => name.NpgsqlDecrypt(Password,Iv,KeyLength));
		private static string Password { get; set; }
		private static string Iv { get; set; }
		private static int KeyLength { get; set; }
		public static ModelBuilder UseEncryptAttribute(this ModelBuilder builder,string password,EncKeyLength encKeyLength)
        {
	        KeyLength = AesUtil.GetKeyLength(encKeyLength);
            AttributeExtension.Password = AesUtil.PasswordFixer(password,KeyLength);
            Iv = AesUtil.IvFixer(password,KeyLength);
            
            foreach (var entityType in builder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties().Where(p => p.PropertyInfo != null))
				{
					var attributes = property.PropertyInfo.GetCustomAttributes(typeof(NpgsqlEncryptAttribute), false);
					if (attributes.Any())
					{
						property.SetValueConverter(_converter);
					}
				}
			}
			return builder;
        }
    }
}
