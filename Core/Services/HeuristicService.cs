namespace Core.Services;

public class HeuristicService : IHeuristicService
{
    /// <summary>
    /// Algoritimo de Dijkstra aplicado para o cenario da aplicação
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public DijkstraResponse DijkstraAlgorithm(DijkstraRequest request)
    {
        var graph = request.Graph;
        var source = request.Source;
        var destination = request.Destination;

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
        {
            throw new BusinessException("É necessario infomar a origem e o destino!");
        }

        if (source == destination)
        {
            throw new BusinessException("A origem e o destino são os mesmos!");
        }

        //Dicionário para armazenar o menor custo de cada lugar
        var costs = new Dictionary<string, int>();
        //Dicionário para rastrear o caminho (predecessores)
        var previous = new Dictionary<string, string>();
        //Conjunto de lugares visitados
        var visited = new HashSet<string>();
        //Fila de prioridade para lugares a serem explorados (min heap com o custo como chave)
        var priorityQueue = new PriorityQueue<string, int>();

        //Inicializa todos os custos como infinito, exceto o lugar de origem
        foreach (var place in graph.Keys)
        {
            costs[place] = int.MaxValue;

            //Também precisamos verificar todos os destinos e adicioná-los, caso ainda não existam no dicionário costs
            foreach (var neighbor in graph[place])
            {
                if (!costs.ContainsKey(neighbor.Item1))
                {
                    costs[neighbor.Item1] = int.MaxValue;
                }
            }
        }
        costs[source] = 0;

        //Adiciona o lugar de origem à fila de prioridade
        priorityQueue.Enqueue(source, 0);

        while (priorityQueue.Count > 0)
        {
            // Remove o lugar com o menor custo
            string currentPlace = priorityQueue.Dequeue();
            visited.Add(currentPlace);

            // Se chegamos ao destino, podemos parar
            if (currentPlace == destination) break;

            // Verifica se o lugar atual tem vizinhos antes de explorar
            if (!graph.ContainsKey(currentPlace) || graph[currentPlace] == null)
            {
                continue; // Pula para o próximo lugar na fila
            }

            // Explora os vizinhos do lugar atual
            foreach (var neighbor in graph[currentPlace])
            {
                string neighborPlace = neighbor.Item1;
                int weight = neighbor.Item2;

                // Se o vizinho ainda não foi visitado
                if (!visited.Contains(neighborPlace))
                {
                    int newCost = costs[currentPlace] + weight;

                    // Atualiza o custo e o caminho se encontrarmos um caminho mais barato
                    if (newCost < costs[neighborPlace])
                    {
                        costs[neighborPlace] = newCost;
                        previous[neighborPlace] = currentPlace;
                        priorityQueue.Enqueue(neighborPlace, newCost);
                    }
                }
            }
        }

        //Reconstrução do caminho a partir do destino
        var path = new List<string>();
        var step = destination;

        if (!previous.ContainsKey(destination) && source != destination)
        {
            //Se não há caminho possível
            return new DijkstraResponse
            {
                Path = new List<string>(),
                TotalCost = int.MaxValue,
                Message = "Não foi encontrato caminho!"
            };
        }

        while (step != null)
        {
            path.Insert(0, step);
            previous.TryGetValue(step, out step);
        }

        return new DijkstraResponse
        {
            Path = path,
            TotalCost = costs[destination],
            Message = "Sucesso!"
        };
    }
}
