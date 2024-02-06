using AutoMapper;

namespace CityInfo.API.Profiles
{
	public class PointsOfInterestProfile : Profile
	{
        public PointsOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
            CreateMap<Models.PointOfInterestForCreationDto  , Entities.PointOfInterest>();
        }
    }
}
