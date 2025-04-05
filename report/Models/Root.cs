using System.Text.Json.Serialization;

namespace report.Models;

public class Root
{
    [JsonPropertyName("energy_per_hours")]
    public required List<double> EnergyByHours { get; set; }
    
    [JsonPropertyName("Дата")]
    public required string Data { get; set; }
}