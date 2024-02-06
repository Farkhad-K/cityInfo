using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route(("api/[controller]"))]
	public class CitiesController : ControllerBase
	{
		private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;

		public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
		{
			_cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDTO>>> GetCities()
		{
			var cityEntities = await _cityInfoRepository.GetCitiesAsync();

			/*var result = new List<CityWithoutPointsOfInterestDTO>();

			foreach (var city in cityEntities)
			{
				result.Add(new CityWithoutPointsOfInterestDTO
				{
					Id = city.Id,
					Name = city.Name,
					Description = city.Descripion
				});
			}

			return Ok(result);*/

			return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(cityEntities));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = true)
		{
			var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);

			if (cityToReturn == null)
			{
				return NotFound();
			}

			if (includePointsOfInterest)
			{
				return Ok(_mapper.Map<CityDto>(cityToReturn));
			}

			return Ok(_mapper.Map<CityWithoutPointsOfInterestDTO>(cityToReturn));
			//return Ok();
		}
	}
}
