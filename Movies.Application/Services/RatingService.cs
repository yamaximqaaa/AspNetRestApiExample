using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMovieRepository _movieRepository;
    
    public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
    {
        _ratingRepository = ratingRepository;
        _movieRepository = movieRepository;
    }
    
    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, 
        CancellationToken cancellationToken = default)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ValidationException([             // this like 'new [] {a, b}'
                new ValidationFailure
                {
                    PropertyName = nameof(rating),
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            ]);
        }
        
        var movieExist = await _movieRepository.ExistedByIdAsync(movieId, cancellationToken);
        if (!movieExist)
            throw new ValidationException([             // this like 'new [] {a, b}'
                new ValidationFailure
                {
                    PropertyName = nameof(movieId),
                    ErrorMessage = "Movie not found"
                }
            ]);

        return await _ratingRepository.RateAsync(
            movieId,
            userId,
            rating,
            cancellationToken);
    }
}