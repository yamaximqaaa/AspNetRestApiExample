using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application.Validator;

public class MovieValidator: AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;
    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Genres)
            .NotEmpty();
        
        // -- This is redundant, in update delete all and create again --
        // RuleFor(x => x.Genres)
        //     .MustAsync(ValidateGenresInDb)
        //     .WithMessage("Try to insert a dublicate genre for this movie");
        
        RuleFor(x => x.Genres)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("Dublicates in genres array");
        
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system");
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken cancellationToken)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug, cancellationToken: cancellationToken);
        if (existingMovie is not null)
        {
            return existingMovie.Id == movie.Id;
        }

        return existingMovie is null;
    }

    private async Task<bool> ValidateGenresInDb(Movie movie, IEnumerable<string> genres,
        CancellationToken cancellationToken)
    {
        var existingGenres = await _movieRepository.ExistedGenres(movie.Id, genres, cancellationToken);
        
        return existingGenres.ToList().Count == 0;
    }
}