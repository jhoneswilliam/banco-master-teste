namespace Domain.Interfaces.Services;

public interface IHeuristicService
{
    DijkstraResponse DijkstraAlgorithm(DijkstraRequest request);
}


