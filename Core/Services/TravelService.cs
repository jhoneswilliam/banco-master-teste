namespace Core.Services;

public class TravelService(ITravelCostFileRepository travelCostRepository, IHeuristicService heuristicService, IMapper mapper) : ITravelService
{
    public async Task<BestCostTravelResponse?> GetBestCostTravel(string departure, string arrival, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(departure) || string.IsNullOrEmpty(arrival))
        {
            throw new BusinessException("É necessario infomar a origem e o destino!");
        }

        if (departure == arrival)
        {
            throw new BusinessException("A origem e o destino são os mesmos!");
        }

        var travelsCosts = await travelCostRepository.GetAllTravels();

        var graph = ConvertToGraph(travelsCosts);

        var bestHeuristicCost = heuristicService.DijkstraAlgorithm(new DijkstraRequest
        {
            Source = departure,
            Destination = arrival,
            Graph = graph
        });

        if (bestHeuristicCost.TotalCost == int.MaxValue)
        {
            return null;
        }

        return new BestCostTravelResponse
        {
            Arrival = arrival,
            Departure = departure,
            Route = string.Join('-', bestHeuristicCost.Path),
            TotalCost = bestHeuristicCost.TotalCost,
        };
    }

    public async Task ImportCostTravel(IList<ImportTravelsCostRequest> importTravelsCosts)
    {
        if (!importTravelsCosts.Any())
        {
            return;
        }

        var travelsCostsToUpsert = mapper.Map<IList<TravelCost>>(importTravelsCosts);

        await travelCostRepository.UpsertBulk(travelsCostsToUpsert);
    }

    private Dictionary<string, List<(string, int)>> ConvertToGraph(IList<TravelCost> travelCosts)
    {
        var graph = new Dictionary<string, List<(string, int)>>();

        foreach (var travelCost in travelCosts)
        {
            if (string.IsNullOrEmpty(travelCost.Departure) || string.IsNullOrEmpty(travelCost.Arrival))
            {
                continue;
            }

            if (!graph.ContainsKey(travelCost.Departure))
            {
                graph[travelCost.Departure] = new List<(string, int)>();
            }

            graph[travelCost.Departure].Add((travelCost.Arrival, travelCost.Cost));
        }

        return graph;
    }

    public void RemoveAllSavedTavelCosts()
    {
        travelCostRepository.RemoveAll();
    }
}
