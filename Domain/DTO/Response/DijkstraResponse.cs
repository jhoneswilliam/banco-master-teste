namespace Domain.DTO.Response;

public record DijkstraResponse
{
    public required List<string> Path { get; set; }
    public required int TotalCost { get; set; }
    public required string Message { get; set; }
}