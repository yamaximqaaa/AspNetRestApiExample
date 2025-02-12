namespace Movies.Application.Repositories;

public interface IRatingRepository
{
    Task<bool> RateAsync(Guid movieId, Guid userId, int rating, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default);
    
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default);
    
    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default);
}