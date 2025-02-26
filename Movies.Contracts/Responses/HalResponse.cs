using System.Text.Json.Serialization;

namespace Movies.Contracts.Responses;

/// <summary>
/// Hypermedia API Language
/// </summary>
public abstract class HalResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<Link> Links { get; set; } = new();
}

public class Link
{
    
    public required string Href { get; set; }
    
    public required string Rel { get; set; }    // Relationship url 
    
    public required string Type { get; set; }
}