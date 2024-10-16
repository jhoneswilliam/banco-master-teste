using Domain.Exceptions;

namespace ConsoleApp;

public class InterfaceConsole(ITravelService travelService)
{
    public async Task Run(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                Console.Out.Write("Digite a rota: ");
                var rota = Console.ReadLine();

                if (rota == null)
                {
                    throw new Exception("Input invalido!");
                }

                var arrRota = rota.Split('-');

                if (arrRota.Length != 2)
                {
                    throw new Exception("Input invalido!");
                }

                var departure = arrRota[0];
                var arrival = arrRota[1];

                var result = await travelService.GetBestCostTravel(departure, arrival, ct);

                Console.WriteLine($"Melhor Rota: {string.Join(" - ", result!.Route)} ao custo de ${result.TotalCost}");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}
