using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService: IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public Task<bool> CreateAsync(Movie movie)
    {
        return _movieRepository.CreateAsync(movie);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return _movieRepository.GetByIdAsync(id);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return _movieRepository.GetBySlugAsync(slug);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return _movieRepository.GetAllAsync();
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        var movieExist = await _movieRepository.UpdateAsync(movie);
        if (!movieExist)
            return null;
        await _movieRepository.UpdateAsync(movie);
        return movie;
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return _movieRepository.DeleteAsync(id);
    }
}