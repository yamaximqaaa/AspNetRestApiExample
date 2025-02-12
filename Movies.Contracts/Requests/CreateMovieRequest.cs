namespace Movies.Contracts.Requests;

public class CreateMovieRequest
{
    // required - needed properties 
    // init - for not changing them 
    public required string Title { get; init; }
    
    public required int YearOfRelease { get; init; }
    
    public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
}