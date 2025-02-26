using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    
    public required string Title { get; set; }

    public string Slug => GenerateSlug();

    public float? Rating { get; set; }
    
    public int? UserRating { get; set; }
    
    public required int YearOfRelease { get; set; }
    public required List<string> Genres { get; init; } = new();

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, String.Empty)
            .ToLower().Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }
    // TODO: Exception with this title name "Once Upon a Time... When We Were Colored" when timeout 5 ms 
    // Uses GeneratedRegex to optimize regex compilation at compile-time.
    // SlugRegex() is a partial method, and its implementation is auto-generated.
    // This regex removes all characters except letters, numbers, spaces, underscores, and hyphens.
    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 10)]
    private static partial Regex SlugRegex();
}