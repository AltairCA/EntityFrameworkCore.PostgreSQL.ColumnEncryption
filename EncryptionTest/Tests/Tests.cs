using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using Bogus;
using EncryptionTest.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EncryptionTest.Tests
{
    public class Tests
    {
        private readonly ApplicationDbContext _dbContext;

        public Tests(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Fact]
        public async Task TEST_INSERT()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            
            
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();
            //Validation
            int count = await _dbContext.Testings.CountAsync();
            Assert.Equal(10,count);
        }

        [Fact]
        public async Task TEST_FETCH()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();
            
            //validation
            foreach (Testings testings in testObjs)
            {
                var obj = await _dbContext.Testings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == testings.Id);
                Assert.NotNull(obj);
                Assert.Equal(obj.encrypted,testings.encrypted);
            }
        }

        [Fact]
        public async Task TEST_DB_FUNCTION()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();
            //validation
            foreach (Testings testings in testObjs)
            {
                var obj = await _dbContext.Testings.AsNoTracking().FirstOrDefaultAsync(x => EF.Functions.NpgsqlDecrypt(x.encrypted) == testings.encrypted);
                Assert.NotNull(obj);
                Assert.Equal(obj.encrypted,testings.encrypted);
            }
        }
        [Fact]
        public async Task TEST_DB_FUNCTION_EXTENSION()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();
            //validation
            foreach (Testings testings in testObjs)
            {
                var obj = await _dbContext.Testings.AsNoTracking().FirstOrDefaultAsync(x => x.encrypted.NpgDecrypt() == testings.encrypted);
                Assert.NotNull(obj);
                Assert.Equal(obj.encrypted,testings.encrypted);
            }
        }

        [Fact]
        public async Task TEST_GROUP_BY()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();

            var grouped = await _dbContext.Testings.AsNoTracking().GroupBy(x => x.encrypted.NpgDecrypt()).Select(x => x.Key)
                .ToListAsync();
            //Test
            foreach (string s in grouped)
            {
                var obs = testObjs.FirstOrDefault(x => x.encrypted == s);
                Assert.NotNull(obs);
                Assert.Equal(obs.encrypted,s);
            }
        }

        [Fact]
        public async Task TEST_MAPPING_NOT_DECRYPTED()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();
            //Test
            var testObjs2 = await _dbContext.Testings.AsNoTracking().Select(x => new 
            {
                Id = x.Id,
                encrypted = x.encrypted.ToUpper(),
                normal = x.normal
            }).ToListAsync();

            foreach (var objs in testObjs2)
            {
                var obj = testObjs.FirstOrDefault(x => x.Id == objs.Id);
                Assert.NotNull(obj);
                Assert.NotEqual(obj.encrypted.ToUpper(),objs.encrypted);
            }
        }
        [Fact]
        public async Task TEST_MAPPING_DECRYPTED()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();
            //Test
            var testObjs2 = await _dbContext.Testings.AsNoTracking().Select(x => new 
            {
                Id = x.Id,
                encrypted = x.encrypted.NpgDecrypt().ToUpper(),
                normal = x.normal
            }).ToListAsync();

            foreach (var objs in testObjs2)
            {
                var obj = testObjs.FirstOrDefault(x => x.Id == objs.Id);
                Assert.NotNull(obj);
                Assert.Equal(obj.encrypted.ToUpper(),objs.encrypted);
            }
        }
        [Fact]
        public async Task TEST_DB_FUNCTION_ENCRYPT()
        {
            //Prepare
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.Database.MigrateAsync();
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            //Execute
            var testObjs = testOrders.Generate(10);
            await _dbContext.Testings.AddRangeAsync(testObjs);
            await _dbContext.SaveChangesAsync();
            //validation
            foreach (Testings testings in testObjs)
            {
                var obj = await _dbContext.Testings.AsNoTracking().FirstOrDefaultAsync(x => x.encrypted == x.normal.NpgEncrypt() && x.Id == testings.Id);
                Assert.NotNull(obj);
                Assert.Equal(obj.encrypted,testings.encrypted);
            }
        }
    }
}