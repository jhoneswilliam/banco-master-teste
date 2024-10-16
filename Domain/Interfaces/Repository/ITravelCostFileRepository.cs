namespace Domain.Interfaces.Repository;

public interface ITravelCostFileRepository
{
    Task<IList<TravelCost>> GetAllTravels();
    Task UpsertBulk(IList<TravelCost> travelsCost);
    void RemoveAll();
}