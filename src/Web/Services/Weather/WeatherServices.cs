using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Web.Domain;
using Web.Infrastructure;
using Web.Infrastructure.Model;
using Web.Messaging.Sender;
using Web.Models.Dtos;
using Web.Models.ViewModels;

namespace Web.Services.Weather
{
    public class WeatherServices : IWeatherServices
    {
        private readonly IHttpClientServices _httpClientServices;
        private readonly IBackgroundJobClient _backgroundJob;
        private readonly IWeatherSender _sender;
        private readonly string _apiKey;
        private readonly RaimunDbContext _context;

        public WeatherServices(IHttpClientServices httpClientServices, IOptions<WeatherApiSetting> option,
            IBackgroundJobClient backgroundJob, IWeatherSender sender, RaimunDbContext context)
        {
            _httpClientServices = httpClientServices;
            _backgroundJob = backgroundJob;
            _sender = sender;
            _context = context;
            _apiKey = option.Value.ApiKey;
        }

        public async Task<WeatherViewModel> GetWeatherByLocation(string location, int hour)
        {
            var response = await _httpClientServices.Get("/current.json?key=" + _apiKey + "&q=" + location);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<WeatherViewModel>(result);
                _backgroundJob.Schedule(() => _sender.Send(new WeatherMessage
                    {
                        Temperature = res.Current.Temp_C,
                        CityName = res.Location.Name
                    }),
                    TimeSpan.FromSeconds(hour));
                return res;
            }

            return null;
        }

        public async Task AddWeather(WeatherDto dto, CancellationToken cancellationToken)
        {
            var weather = new Domain.Weather
            {
                Location = dto.CityName,
                Temperature = dto.Temperature
            };
            await _context.Weathers.AddAsync(weather, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public interface IWeatherServices
    {
        Task<WeatherViewModel> GetWeatherByLocation(string location, int hour);
        Task AddWeather(WeatherDto dto, CancellationToken cancellationToken);
    }
}