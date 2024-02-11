using AutoMapper;

namespace CityInfo.API.Profiles
{
	public class CityProfile : Profile
	{
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDTO>()
                //this adds nullabel properties to mapper
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Descripion));
            CreateMap<Entities.City, Models.CityDto>()
				.ForMember(d => d.Description, opt => opt.MapFrom(src => src.Descripion));
		}
    }
}
