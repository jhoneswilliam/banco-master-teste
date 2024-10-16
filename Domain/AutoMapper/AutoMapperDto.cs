
namespace Domain.AutoMapper;

public class AutoMapperDto : Profile
{
    public AutoMapperDto()
    {
        #region Request
        CreateMap<ImportTravelsCostRequest, TravelCost>();
        #endregion

        #region Response
        CreateMap<TravelCost, BestCostTravelResponse>();
        #endregion
    }
}
