using AutoMapper;

namespace CityInfo.API.Profiles
{
	public class PointsOfInterestProfile : Profile
	{
        public PointsOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();
                //.ForMember(d => d.Description, opt => opt.MapFrom(src => src.City.Descripion));
			CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();
            CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>();
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDto>();
        }
    }
}
