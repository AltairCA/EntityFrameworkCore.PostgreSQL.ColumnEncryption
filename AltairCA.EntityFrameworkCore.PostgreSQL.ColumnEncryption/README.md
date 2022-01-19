# AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption

AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption is a NPGSQL Extension that supports native PostgreSql's 
[Raw Encryption Functions (encrypt_iv,decrypt_iv with aes-cbc/pad:pkcs)](https://www.postgresql.org/docs/10/pgcrypto.html) (see section F.26.4. Raw Encryption Functions). 
Meaning this will support search query on encrypted columns. Well, this is good if you have GDPR compliance requirement.

### Note
`If you use this make sure your application to the PostgreSQL service is use a encrypted connection because this will transmit the RAW PASSWORD over the network`. You can enforce it in the connection string. example -
```json
{
  "DefaultConnection": "Server=127.0.0.1;port=5432;user id=postgres;password=postgres;database=AESTester;pooling=true;Encoding=UTF8;SSL Mode=Require;Trust Server Certificate=true;"
}
```
take a look at `SSL Mode=Require;Trust Server Certificate=true;`

`AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption` targets `net6.0`. The package has following dependencies

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="6.0.1" />
```

When you choose the version choose with the `.Net Core` Version for example if `.Net Core` version is 5.0 then choose `AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption` version 5.0.x

## setup

### NuGet install:

`Install-Package AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption`

### DbContext

```c#
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseEncryptionFunctions("yourpassword",EncKeyLength.L128);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("pgcrypto");
            builder.UseEncryptAttribute("yourpassword",EncKeyLength.L128);
        }
```

Replace `yourpassword` with your password

### Example of use

#### `NpgsqlEncrypt` Annotation Use
```c#
    public class TestModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [NpgsqlEncrypt] #Use NpgsqlEncrypt attribute to denote the property must be encrypt in database
        public string Name { get; set; }
    }
```
#### How to use it in search query
```c#
var searchTest = _dbContext.TModels.Where(x => x.Name.NpgDecrypt().Contains("test")).ToList();
```

Above Linq will convert to a Native Sql Query that will decrypt the column before it do a search.

You can find a example that I have used in the `EncryptionTest` project `Tests\Tests.cs` and `WebApplication` Project `WeatherForecastController.cs`