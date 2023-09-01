using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointOfInterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _cityDataStore;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger, 
            IMailService mailService,
            CitiesDataStore citiesDataStore
         ) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService;
            _cityDataStore = citiesDataStore;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var result = GetCity(cityId);

                if (result == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                    return NotFound();
                }

                return Ok(result.PointsOfInterest);
            } 
            catch (Exception ex) 
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}.", ex);

                return StatusCode(500, "A problem happened while handling your request");
            }
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

            var city = GetCity(cityId);

            if (city == null) return NotFound();

            var maxPointOfInterest = _cityDataStore.Cities.SelectMany(
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

        [HttpPut("{pointOfInterestId}")]
        public ActionResult<PointOfInterestUpdateDto> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestUpdateDto pointOfInterest)
        {
            var city = GetCity(cityId);

            if(city == null) return NotFound();

            // find point of interest
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id== pointOfInterestId);

            if (pointOfInterestFromStore== null) return NotFound();

            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();

        }

        [HttpDelete("{pointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = GetCity(cityId);

            if (city == null) return NotFound();

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

            if (pointOfInterestFromStore== null) return NotFound();

            city.PointsOfInterest.Remove(pointOfInterestFromStore);
            _mailService.Send("Point of interest deleted",
                $"Point of intrest {pointOfInterestFromStore.Name} with id {pointOfInterestId}");

            return NoContent();

        }


        // Partially updates a resource
        /*
         Sample request body parameter for PATCH

            [
                {
                    "op": "replace",
                    "patch": "/invalidproperty",
                    "value": "Updated - Central Park"
                }
            ]

         */
        [HttpPatch("{pointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            var city = GetCity(cityId); 
            
            if(city == null) return NotFound();

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);

            if (pointOfInterestFromStore == null) return NotFound();

            var pointOfInterestToPatch = new PointOfInterestUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description,
            };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            // Model State -> Dictionary containing both the state of the model and
            // model binding validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // This triggers validation of our model and any errors
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }
        private CityDto GetCity(int id)
        {
            return _cityDataStore.Cities.FirstOrDefault(c => c.Id == id);
        }
    }
}
