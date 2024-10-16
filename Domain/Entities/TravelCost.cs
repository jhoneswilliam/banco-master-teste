namespace Domain.Entities;

public record TravelCost
{
    public required string? Departure { get; set; }
    public required string? Arrival { get; set; }
    public required int Cost { get; set; }
}
