namespace Domain.DTO.Request;

public record ImportTravelsCostRequest
{
    public required string? Departure { get; set; }
    public required string? Arrival { get; set; }
    public required int Cost { get; set; }
}
