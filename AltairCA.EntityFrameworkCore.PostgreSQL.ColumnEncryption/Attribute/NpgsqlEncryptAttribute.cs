using System;

namespace AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Attribute
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class NpgsqlEncryptAttribute:System.Attribute
    {
        
    }
}