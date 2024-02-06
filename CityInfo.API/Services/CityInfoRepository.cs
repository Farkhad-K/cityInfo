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

		public async Task<IEnumerable<City>> GetCitiesAsync()
		{
			return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
		}

		// Returns true if city exists
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
	}
}
	
