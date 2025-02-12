using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnection;

    public RatingRepository(IDbConnectionFactory dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<bool> RateAsync(Guid movieId, Guid userId, int rating, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
             insert into ratings (movieid, userid, rating)
             values (@movieId, @userId, @rating)
             on conflict (movieid, userid) do update 
                 set rating = @rating
             """, new { movieId, userId, rating }, cancellationToken: cancellationToken));
        return result > 0;
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from ratings where movieid = @movieId and userid = @userId
            """, new { movieId, userId }, cancellationToken: cancellationToken));
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition(
            """
            select round(avg(r.rating), 1) from ratings r
            where movieId = @movieId
            """, new { movieId }, cancellationToken: cancellationToken));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition(
            """
            select round(avg(r.rating), 1), (select rating from ratings 
                                                where movieId = @movieId 
                                                and userId = @userId
                                                limit 1) 
            from ratings r
            where movieId = @movieId
            """, new { movieId, userId }, cancellationToken: cancellationToken));
    }
}