using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Authorize]
	[ApiVersion("1.0")]
	[ApiVersion("2.0")]
	[Route(("api/v{version:apiVersion}/[controller]"))]
	public class CitiesController : ControllerBase
	{
		private readonly ICityInfoRepository _cityInfoRepository;
		private readonly IMapper _mapper;
		const int maxCitiesPageSize = 20;

		public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
		{
			_cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDTO>>> GetCities(string? name, string? searchQuery,
			int pageNumber = 1, int pageSize = 10)
		{
			if(pageSize > maxCitiesPageSize) { pageSize = maxCitiesPageSize; }



			var (cityEntities, paginationMetadata) = await _cityInfoRepository
				.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

			Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
			
			return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDTO>>(cityEntities));
		}


		// For adding xml commets(documentation) for the actions or DTOs see session - 10, video - 7 and 8.
		/// <summary>
		/// Get city by id
		/// </summary>
		/// <param name="id">The id for the city to get</param>
		/// <param name="includePointsOfInterest">Whether or not to include the points of interest</param>
		/// <returns>An IActionResult</returns>
		/// <response code="200">Returns the requested city</response>
		/// <response code="400">Returns badass request</response>
		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
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
