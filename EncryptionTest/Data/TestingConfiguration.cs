using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.EfExtension;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EncryptionTest.Data;

public class TestingConfiguration:IEntityTypeConfiguration<Testings>
{
    public void Configure(EntityTypeBuilder<Testings> builder)
    {
        builder.Property(x => x.encrypted)
            .UseEncryption("NBJ42RKQ2vQoYFZOj1C83921vHExVhVp1PlOAK6gjbMZI", EncKeyLength.L128);
    }
}