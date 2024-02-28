using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService: IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
    {
        _movieRepository = movieRepository;
        _movieValidator = movieValidator;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);
        return await _movieRepository.CreateAsync(movie, cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetBySlugAsync(slug, cancellationToken);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);
        var movieExist = await _movieRepository.UpdateAsync(movie, cancellationToken);
        if (!movieExist)
            return null;
        await _movieRepository.UpdateAsync(movie, cancellationToken);
        return movie;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteAsync(id, cancellationToken);
    }
}