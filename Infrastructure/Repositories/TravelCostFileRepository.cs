namespace Infrastructure.Repositories;

public class TravelCostFileRepository : ITravelCostFileRepository
{
    private readonly string filePath;

    public TravelCostFileRepository(IConfiguration appConfiguration)
    {
        if (string.IsNullOrEmpty(appConfiguration["FileName"]))
        {
            throw new ArgumentException("O paramentro FileName não está definido no arquivo de configurações");
        }

        filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appConfiguration["FileName"]!);
    }

    public async Task<IList<TravelCost>> GetAllTravels()
    {
        if (!File.Exists(filePath))
        {
            return new List<TravelCost>();
        }

        var lines = await File.ReadAllLinesAsync(filePath);
        var travelCosts = new List<TravelCost>();

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length == 3)
            {
                travelCosts.Add(new TravelCost
                {
                    Departure = parts[0],
                    Arrival = parts[1],
                    Cost = int.Parse(parts[2])
                });
            }
        }
        return travelCosts;
    }

    public void RemoveAll()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async Task UpsertBulk(IList<TravelCost> travelsCost)
    {
        var currentTravels = await GetAllTravels() ?? new List<TravelCost>();

        var travelDictionary = new Dictionary<(string? Departure, string? Arrival), TravelCost>();

        foreach (var travel in currentTravels)
        {
            var key = (travel.Departure, travel.Arrival);
            travelDictionary[key] = travel;
        }

        foreach (var travel in travelsCost)
        {
            var key = (travel.Departure, travel.Arrival);
            if (travelDictionary.ContainsKey(key))
            {
                travelDictionary[key] = travel;
            }
            else
            {
                travelDictionary[key] = travel;
            }
        }

        var lines = new List<string>();
        foreach (var travel in travelDictionary.Values)
        {
            lines.Add($"{travel.Departure},{travel.Arrival},{travel.Cost}");
        }

        await File.WriteAllLinesAsync(filePath, lines);
    }
}