namespace Domain.Interfaces.Services;

public interface ITravelService
{
    Task<BestCostTravelResponse?> GetBestCostTravel(string departure, string arrival, CancellationToken ct);
    Task ImportCostTravel(IList<ImportTravelsCostRequest> travelsCosts);
    void RemoveAllSavedTavelCosts();
}

