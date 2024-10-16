namespace Domain.DTO.Request;

public record DijkstraRequest
{
    public required Dictionary<string, List<(string, int)>> Graph { get; set; }
    public required string Source { get; set; }
    public required string Destination { get; set; }
}