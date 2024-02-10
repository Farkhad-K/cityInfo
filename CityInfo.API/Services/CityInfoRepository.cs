using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
	public class CityInfoRepository : ICityInfoRepository
	{
		private readonly CityInfoContext _context;

		public CityInfoRepository(CityInfoContext context)
		{
			this._context = context;
		}

		public async Task<City?> GetCityAsync(int cityId, bool pointsOfInterest)
		{
			if (pointsOfInterest)
			{
				return await _context.Cities
					.Include(c => c.PointsOfInterest)
					.FirstOrDefaultAsync(c => c.Id == cityId);
			}

			return await _context.Cities
				.FirstOrDefaultAsync(c => c.Id == cityId);
		}

		public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
		{
			return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
		}

		public async Task<IEnumerable<City>> GetCitiesAsync()
		{
			return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
		}


		/// <summary>
		/// This method Filters and Searches through Cities based on the "name" and "searchQuery" parameters.
		/// </summary>
		/// <param name="name">This parameter applies for Filtering</param>
		/// <param name="searchQuery">This parameter applies for Searching</param>
		/// <returns>Based on the request returns Filtered collection</returns>
		/// <returns>Based on the request Searches for stirng which is located in the "searchQuery"</returns>
		/// <example>
		/// /api/cities?name=Antwerp this is how Filtering is appied
		/// </example>
		/// <example>
		/// /api/cities?searchQuery=the this is how Searching is appied
		/// </example>

		public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
		{
			// This collection helps when both serching and filtering applies
			// And this collection also helps to execute Searching and Filtering at the data base level
			// not after the the records has been fetched and this called Defered Execution
			var collection = _context.Cities as IQueryable<City>;

			if (!string.IsNullOrWhiteSpace(name))
			{
				name = name.Trim();
				collection = collection.Where(c => c.Name == name);
			}

			if (!string.IsNullOrWhiteSpace(searchQuery))
			{
				searchQuery = searchQuery.Trim();
				collection = collection.Where(a => a.Name.Contains(searchQuery)
				|| (a != null && a.Descripion.Contains(searchQuery)));
			}

			var totalItemCount = await collection.CountAsync();

			var paginationMetadate = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

			var collectionToReturn = await collection.OrderBy(c => c.Name)
				.Skip(pageSize * (pageNumber - 1))
				.Take(pageSize)
				.ToListAsync();

			return (collectionToReturn, paginationMetadate);
		}

		/// <summary>
		/// Checks if the city exists
		/// </summary>
		/// <param name="cityId"></param>
		/// <returns>Returns true if city exists</returns>
		public async Task<bool> CityExistsAsync(int cityId)
		{
			return await _context.Cities.AnyAsync(c => c.Id == cityId);
		}

		public async Task<PointOfInterest> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
		{
			//return await _context.PointsOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
			return await _context.PointsOfInterests
				.FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == pointOfInterestId);
		}

		public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
		{
			return await _context.PointsOfInterests
				.Where(p => p.CityId == cityId)
				.ToListAsync();
		}

		public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
		{
			var city = await GetCityAsync(cityId, false);

			if (city != null)
			{
				city.PointsOfInterest.Add(pointOfInterest);
			}
		}

		public async Task<bool> SaveChangesAsync()
		{
			// SaveChangesAsync() this method returns amount of the entities that have been changed
			// ">= 0" this signature makes this method true even when we change 0 or more entities
			return await _context.SaveChangesAsync() >= 0;
		}

		public void DeletePointOfInterest(PointOfInterest pointOfInterest)
		{
			_context.PointsOfInterests.Remove(pointOfInterest);
		}
	}
}

