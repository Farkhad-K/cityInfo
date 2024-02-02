using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityId}/pointsofinterest")]
	[ApiController]
	public class PointsOfInterestsController : ControllerBase
	{
		private readonly ILogger<PointsOfInterestsController> _logger;
		private readonly IMailService _mailService;
		private readonly CitiesDataStore _citiesDataStore;

		public PointsOfInterestsController(ILogger<PointsOfInterestsController> logger, IMailService mailService, 
			CitiesDataStore citiesDataStore)
        {
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
			_citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
		}

        [HttpGet]
		public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
		{
			try
			{
				// throw new Exception("Exception sample.");

				var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);

				if (city == null)
				{
					_logger.LogInformation($"City with id {cityId} was not found when accessing point of interest.");
					return NotFound();
				}

				return Ok(city.PointsOfInterest);
			}
			catch (Exception ex)
			{
				_logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex.Message);
				return StatusCode(500, "A problem happened while handling your request.");
			}
		}

		[HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
		public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
		{
			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterest == null)
			{
				return NotFound();
			}

			return Ok(pointOfInterest);
		}

		[HttpPost]
		public ActionResult<PointsOfInterestsController> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
		{


			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var maxPointOfInterestId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

			var finalPointOfInterest = new PointOfInterestDto()
			{
				Id = ++maxPointOfInterestId,
				Name = pointOfInterest.Name,
				Description = pointOfInterest.Description
			};

			city.PointsOfInterest.Add(finalPointOfInterest);

			return CreatedAtRoute("GetPointOfInterest",
				new
				{
					cityId = cityId,
					pointOfInterestId = finalPointOfInterest.Id
				},
				finalPointOfInterest);
		}

		[HttpPut("{pointofinterestid}")]
		public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
		{
			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null) return NotFound();

			var currentPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (currentPointOfInterest == null) return NotFound();

			currentPointOfInterest.Name = pointOfInterest.Name;
			currentPointOfInterest.Description = pointOfInterest.Description;

			return NoContent();

			/*//var finalPointOfInterest = currentPointOfInterest.Equals(
			//	new PointOfInterestForUpdateDto()
			//	{
			//		Name = pointOfInterest.Name,
			//		Description = pointOfInterest.Description
			//	});

			//return Ok(finalPointOfInterest);*/
		}

		[HttpPatch("{pointofinterestid}")]
		public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
			JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
		{
			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null) return NotFound();

			var currentPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (currentPointOfInterest == null) return NotFound();

			var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
			{
				Name = currentPointOfInterest.Name,
				Description = currentPointOfInterest.Description
			};

			patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

			if(!ModelState.IsValid) return BadRequest(ModelState);

			if(!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

			currentPointOfInterest.Name = pointOfInterestToPatch.Name;
			currentPointOfInterest.Description = pointOfInterestToPatch.Description;

			return NoContent();
		}

		[HttpDelete("{pointofinterestid}")]
		public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
		{
			var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null) return NotFound();

			var currentPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (currentPointOfInterest == null) return NotFound();

			city.PointsOfInterest.Remove(currentPointOfInterest);
			_mailService.Send("Points of interest is deleted", $"Points of interest {currentPointOfInterest.Name} with id {currentPointOfInterest.Id} was deleted");

			return NoContent();
		}
	}
}
