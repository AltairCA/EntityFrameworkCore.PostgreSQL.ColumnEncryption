using System;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Attribute;

namespace EncryptionTest.Data
{
    public class Testings
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [NpgsqlEncrypt]
        public string encrypted { get; set; }
        public string normal { get; set; }
    }
}