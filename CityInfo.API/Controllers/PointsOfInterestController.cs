using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointOfInterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var result = GetCity(cityId);

            if (result == null) return NotFound();

            return Ok(result.PointsOfInterest);
        }

        // Added Name so we can easily refer to it when calling into CreatedAtRoute.
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = GetCity(cityId);

            if (city == null) return Ok(new List<PointOfInterestDto>());

            var result = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);

            return Ok(result);
        }

        [HttpPost]
        public ActionResult<PointOfInterestCreationDto> CreatePointOfInterest(int cityId, PointOfInterestCreationDto pointOfInterest)
        {

            // Model state is a dictionary containing both the state of the model ( PointOfInterestCreationDto ),

            if (!ModelState.IsValid) return BadRequest();

            var city = GetCity(cityId);

            if (city == null) return NotFound();

            var maxPointOfInterest = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterest,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            // For post, the advices respoinse is 201 Created
            // CreatedAtRoute -> allows us to return a response with the location header
            // That location header will contain the URI where the newly created point of interest
            //  can be found.

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                }, finalPointOfInterest);
        }

        private CityDto GetCity(int id)
        {
            return CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
        }
    }
}
