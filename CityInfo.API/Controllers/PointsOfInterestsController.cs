using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityId}/pointsofinterest")]
	[ApiController]
	public class PointsOfInterestsController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

			if (city == null)
			{
				return NotFound();
			}

			return Ok(city.PointsOfInterest);
		}

		[HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
		public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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


			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

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
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
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
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null) return NotFound();

			var currentPointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (currentPointOfInterest == null) return NotFound();

			city.PointsOfInterest.Remove(currentPointOfInterest);

			return NoContent();
		}
	}
}
