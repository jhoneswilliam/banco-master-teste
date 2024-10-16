namespace ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            cts.Cancel();
            eventArgs.Cancel = true;
        };

        var travelCostsFromFile = GetAllTravelCostsFromFile(args[0]);

        var serviceCollection = new ServiceCollection();
        ConfigureDI(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var travelService = scope.ServiceProvider.GetService<ITravelService>()!;
            travelService.RemoveAllSavedTavelCosts();
            travelService.ImportCostTravel(travelCostsFromFile);

            var interfaceConsole = scope.ServiceProvider.GetService<InterfaceConsole>()!;
            interfaceConsole.Run(cts.Token).Wait();
        }
    }

    private static void ConfigureDI(IServiceCollection services)
    {
        var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddSingleton(new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperDto());
        }).CreateMapper());

        services.AddScoped<ITravelCostFileRepository, TravelCostFileRepository>();
        services.AddScoped<IHeuristicService, HeuristicService>();
        services.AddScoped<ITravelService, TravelService>();
        services.AddScoped<InterfaceConsole>();
    }

    private static List<ImportTravelsCostRequest> GetAllTravelCostsFromFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            throw new Exception($"Arquivo {filepath} não encontrado!");
        }

        using (var stream = File.OpenRead(filepath))
        {
            byte[] buffer = new byte[3];
            stream.Read(buffer, 0, 3);

            if (!(buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF))
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var travelCosts = new List<ImportTravelsCostRequest>();
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var columns = line.Split(',');

                    if (columns.Length != 3 ||
                        !int.TryParse(columns[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var cost))
                    {
                        throw new Exception($"Invalid format in line: {line}");
                    }

                    var travelCost = new ImportTravelsCostRequest
                    {
                        Departure = columns[0],
                        Arrival = columns[1],
                        Cost = cost
                    };

                    travelCosts.Add(travelCost);
                }
            }

            return travelCosts;
        }
    }
}
