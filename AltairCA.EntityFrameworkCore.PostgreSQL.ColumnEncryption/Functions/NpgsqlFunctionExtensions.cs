using System;
using Microsoft.EntityFrameworkCore;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions
{
    public static class NpgsqlFunctionExtensions
    {
        public static string NpgsqlEncrypt(this DbFunctions _,string value)
        {
            throw new InvalidOperationException(
                "This method is for use with Entity Framework Core PgSql only.");
        }

        public static string NpgsqlDecrypt(this DbFunctions _,string value )
        {
            throw new InvalidOperationException(
                "This method is for use with Entity Framework Core PgSql only.");
        }
        public static string NpgEncrypt(this string value)
        {
            throw new InvalidOperationException(
                "This method is for use with Entity Framework Core PgSql only.");
        }
        public static string NpgDecrypt(this string value)
        {
            throw new InvalidOperationException(
                "This method is for use with Entity Framework Core PgSql only.");
        }
        
    }
}
