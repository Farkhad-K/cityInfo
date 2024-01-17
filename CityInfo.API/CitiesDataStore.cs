using CityInfo.API.Models;

namespace CityInfo.API
{
	public class CitiesDataStore
	{
		public List<CityDto> Cities { get; set; }
		public static CitiesDataStore Current { get; } = new CitiesDataStore();

		public CitiesDataStore()
		{
			// init dummy data
			Cities = new List<CityDto>()
			{
				new CityDto()
				{
					Id = 1,
					Name = "New York City",
					Description = "The one with that big park",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 1,
							Name = "Central Park",
							Description = "The most visited urban park in the USA."
						},
						new PointOfInterestDto()
						{
							Id = 2,
							Name = "Empire State Building",
							Description = "A 102-story skyscraper located in Midtown Manhattan."
						}
					}
				},
				new CityDto()
				{
					Id = 2,
					Name = "Rome",
					Description = "The one with that circle building",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 3,
							Name = "Cathedral of Our Lady",
							Description = "A Gothic style catedral, concieved by architects Jan and Piete."
						},
						new PointOfInterestDto()
						{
							Id = 4,
							Name = "Antwerp Central Station",
							Description = "The finest example of railway architecture in Belgium."
						}
					}
				},
				new CityDto()
				{
					Id = 3,
					Name = "Paris",
					Description = "The one with that big tower",
					PointsOfInterest = new List<PointOfInterestDto>()
					{
						new PointOfInterestDto()
						{
							Id = 5,
							Name = "Eiffel Tower",
							Description = "A wrought iron tower on the Chmp de Mars."
						},
						new PointOfInterestDto()
						{
							Id = 6,
							Name = "The Louver",
							Description = "The world's largest museum."
						}
					}
				}
			};
		}
	}
}
