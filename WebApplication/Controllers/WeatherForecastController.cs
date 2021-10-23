using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltairCA.EntityFrameworkCore.PostgreSQL.ColumnEncryption.Functions;
using Bogus;
using EncryptionTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var testOrders = new Faker<Testings>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.encrypted, f => f.Person.FullName)
                .RuleFor(x => x.normal, f => f.Person.FullName);
            
            var obj = await _dbContext.Testings.AsNoTracking().FirstOrDefaultAsync(x => x.encrypted == x.normal.NpgEncrypt());
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}