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
	 = new ValueConverter<string, string>(color => color.NpgsqlEncrypt(Password),
										 name => name.NpgsqlDecrypt(Password));
		private static string Password { get; set; }
		public static ModelBuilder UseEncryptAttribute(this ModelBuilder builder,string password)
        {

            AttributeExtension.Password = AesUtil.PasswordFixer(password);;
            foreach (var entityType in builder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
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
