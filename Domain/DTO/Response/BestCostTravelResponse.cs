namespace Domain.DTO.Response;

public record BestCostTravelResponse
{
    [JsonPropertyName("Origem")]
    public required string? Departure { get; set; }

    [JsonPropertyName("Destino")]
    public required string? Arrival { get; set; }

    [JsonPropertyName("Rota")]
    public required string? Route { get; set; }

    [JsonPropertyName("CustoTotal")]
    public required int TotalCost { get; set; }
}
