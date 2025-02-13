using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepositoryDb: IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnection;

    public MovieRepositoryDb(IDbConnectionFactory dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        
        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            insert into movies (id, slug, title, yearofrelease)
            values (@Id, @Slug, @Title, @YearOfRelease)
            """, movie, cancellationToken: cancellationToken));
        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                    """, 
                    new { MovieId = movie.Id, Name = genre }, 
                    cancellationToken: cancellationToken));
            }
        }
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
                                  from movies m
                                  left join ratings r on m.id = r.movieid
                                  left join ratings myr on m.id = myr.movieid
                                    and myr.userId = @userId
                                  where id = @id
                                  group by id, userrating
                                  """, new { id, userId }, 
                cancellationToken: cancellationToken));
        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
            """
            select name from genres where movieid = @id
            """, new { id }, cancellationToken: cancellationToken));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre); 
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
                                  from movies m
                                  left join ratings r on m.id = r.movieid
                                  left join ratings myr on m.id = myr.movieid
                                    and myr.userId = @userId
                                  where slug = @slug
                                  group by id, userrating
                                  """, new { slug, userId }, 
                cancellationToken: cancellationToken));
        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
                """
                select name from genres where movieid = @id
                """, new { id = movie.Id }, 
                cancellationToken: cancellationToken));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre); 
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync(
            new CommandDefinition("""
                  select m.*, 
                         string_agg( distinct g.name, ',') as genres, 
                         round(avg(r.rating), 1) as rating, 
                         myr.rating as userrating
                  from movies m 
                  left join genres g on m.id = g.movieid
                  left join ratings r on m.id = r.movieid
                  left join ratings myr on m.id = myr.movieid
                        and myr.userId = @userId
                  where (@title is null or m.title like ('%' || @title || '%'))
                  and (@year is null or m.yearofrelease = @year)
                  group by id, userrating
                  """, new { userId = options.UserId, 
                            title = options.Title, 
                            year = options.Year },
                cancellationToken: cancellationToken)
            );

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, 
            new { id = movie.Id }, cancellationToken: cancellationToken));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync("""
                                          insert into genres (movieid, name)
                                          values (@MovieId, @Name)
                                          """, new { MovieId = movie.Id, Name = genre });
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
             update movies set slug = @Slug, title = @Title, yearofrelease = @YearOfRelease
             where id = @Id
             """,
            movie, cancellationToken: cancellationToken));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genres where movieid = @id
                                                            """, 
            new { id }, cancellationToken: cancellationToken));
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from movies where id = @id
                                                                         """, 
            new { id }, cancellationToken: cancellationToken));
        
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            """
            select count(1) from movies where id = @id
            """, new { id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<string>> ExistedGenres(Guid movieId, IEnumerable<string> genres, 
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnection.CreateConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync<string>(new CommandDefinition(
            """
            select name from genres where movieid = @id and name = any(@genresList)
            """, new { id = movieId, genresList = genres.ToList() }, cancellationToken: cancellationToken));
        
        return result;
    }
}