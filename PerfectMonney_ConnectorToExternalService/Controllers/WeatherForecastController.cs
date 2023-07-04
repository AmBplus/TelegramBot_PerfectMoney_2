using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PerfectMonney_ConnectorToExternalService.Settings;

namespace PerfectMonney_ConnectorToExternalService.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IOptions<BotSettings> options)
        {
            _logger = logger;
            Options = options;
        }

        public IOptions<BotSettings> Options { get; }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get([FromServices] IWritableOptions<BotSettings> writableOptions)
        {
            writableOptions.Update(x => x.AboutUs = "ب9321325ب");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}