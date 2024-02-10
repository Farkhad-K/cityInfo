using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Authorize(Policy = "MustBeFromAntwerp")]
	[Route("api/cities/{cityId}/pointsofinterest")]
	public class PointsOfInterestsController : ControllerBase
	{
		private readonly ILogger<PointsOfInterestsController> _logger;
		private readonly IMailService _mailService;
		private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;

		public PointsOfInterestsController(ILogger<PointsOfInterestsController> logger, IMailService mailService,
			ICityInfoRepository cityInfoRepository, IMapper mapper)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));

			_cityInfoRepository = cityInfoRepository
				?? throw new ArgumentNullException(nameof(cityInfoRepository));

			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
		{
			// This returns a name of the city from Authenticatoion controller
			// ValidateUserCredentials() which was specified in this method
			var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

			// This line of code checks if the city from previouse code matches with the city from which was Authentication
			// example: if the Authentication was form Antwerp the response will be only related to the Antwerp
			//if (!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId)) { return Forbid(); }

			if (!await _cityInfoRepository.CityExistsAsync(cityId))
			{
				_logger.LogInformation($"City with an id {cityId} wasn't found.");
				return NotFound();
			}

			var pointsOfInterestForCity = await _cityInfoRepository
				.GetPointsOfInterestForCityAsync(cityId);

			return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
		}

		[HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
		public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId,
			int pointOfInterestId)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
			{
				_logger.LogInformation($"City with an id {cityId} wasn't found.");
				return NotFound();
			}

			var pointOfInterest = await _cityInfoRepository
				.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

			if (pointOfInterest == null)
			{
				return NotFound();
			}

			return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
		}

		[HttpPost]
		public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
			{
				return NotFound();
			}

			var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

			await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

			await _cityInfoRepository.SaveChangesAsync();

			var createdPointOfInterest = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

			return CreatedAtRoute("GetPointOfInterest",
				new
				{
					cityId = cityId,
					pointOfInterestId = finalPointOfInterest.Id
				},
				createdPointOfInterest);
		}

		[HttpPut("{pointofinterestid}")]
		public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
			{
				return NotFound();
			}

			var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
			if (pointOfInterestEntity == null) return NotFound();

			_mapper.Map(pointOfInterest, pointOfInterestEntity);

			await _cityInfoRepository.SaveChangesAsync();

			return NoContent();
		}

		[HttpPatch("{pointofinterestid}")]
		public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
			JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
			{
				return NotFound();
			}

			var pointOfInterestEntity = await _cityInfoRepository
				.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
				return NotFound();
            }

			var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

			patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

			_mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

			await _cityInfoRepository.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{pointofinterestid}")]
		public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
		{
			if (!await _cityInfoRepository.CityExistsAsync(cityId))
			{
				return NotFound();
			}

			var pointOFInterestEntity = await _cityInfoRepository
				.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
			if (pointOFInterestEntity == null)
			{
				return NotFound();
			}

			_cityInfoRepository.DeletePointOfInterest(pointOFInterestEntity);

			await _cityInfoRepository.SaveChangesAsync();

			_mailService.Send("Points of interest is deleted", $"Points of interest {pointOFInterestEntity.Name} with id {pointOFInterestEntity.Id} was deleted");

			return NoContent();
		}
	}
}
