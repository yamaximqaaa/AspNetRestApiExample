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

    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _dbConnection.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        var result = await connection.ExecuteAsync(new CommandDefinition(
            """
            insert into movies (id, slug, title, yearofrelease)
            values (@Id, @Slug, @Title, @YearOfRelease)
            """, movie));
        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                    """, new { MovieId = movie.Id, Name = genre }));
            }
        }
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnection.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select * from movies where id = @id
                                  """, new { id }));
        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
            """
            select name from genres where movieid = @id
            """, new { id }));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre); 
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnection.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select * from movies where slug = @slug
                                  """, new { slug }));
        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition(
                """
                select name from genres where movieid = @id
                """, new { id = movie.Id }));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre); 
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _dbConnection.CreateConnectionAsync();
        var result = await connection.QueryAsync(
            new CommandDefinition("""
                                  select m.*, string_agg(g.name, ',') as genres
                                  from movies m left join genres g on m.id = g.movieid
                                  group by id
                                  """)
            );

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _dbConnection.CreateConnectionAsync();
        var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
            """, new { id = movie.Id }));

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
             """, movie));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = await _dbConnection.CreateConnectionAsync();
        var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            delete from genres where movieid = @id
                                                            """, new { id }));
        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         delete from movies where id = @id
                                                                         """, new { id }));
        
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistedByIdAsync(Guid id)
    {
        using var connection = await _dbConnection.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            """
            select count(1) from movies where id = @id
            """, new { id }));
    }
}