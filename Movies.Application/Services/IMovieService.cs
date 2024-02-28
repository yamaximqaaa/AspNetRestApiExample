using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken);
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken);
    Task<Movie?> UpdateAsync(Movie movie, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}