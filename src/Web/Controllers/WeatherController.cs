using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure.Api;
using Web.Models.Dtos;
using Web.Services.Weather;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherServices _weatherServices;

        public WeatherController(IWeatherServices weatherServices)
        {
            _weatherServices = weatherServices;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">Name of region or country or city or...</param>
        /// <param name="hour">set this time for create job</param>
        /// <returns>Information about temperature of location and etc.</returns>
        [HttpGet]
        public async Task<ApiResult<dynamic>> GetWeatherByLocation([Required] string location, [Required] int hour)
        {
            var result = await _weatherServices.GetWeatherByLocation(location, 1);
            if (result == null)
                return Ok("City Not Found");
            return result;
        }

        [HttpPost]
        public async Task<ApiResult> AddWeather(WeatherDto dto, CancellationToken cancellationToken)
        {
            await _weatherServices.AddWeather(dto, cancellationToken);
            return Ok();
        }
    }
}