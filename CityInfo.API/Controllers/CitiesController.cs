using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly CitiesDataStore _cityDataStore;
        public CitiesController(CitiesDataStore cityDataStore) 
        {
            _cityDataStore = cityDataStore;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_cityDataStore.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var city = _cityDataStore.Cities.FirstOrDefault(x => x.Id == id);

            if (city == null)
            {
                return NotFound("City not found.");
            }

            return Ok(city);
        }
    }
}
