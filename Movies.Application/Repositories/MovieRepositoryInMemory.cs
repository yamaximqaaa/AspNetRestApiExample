using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepositoryInMemory: IMovieRepository
{
    private List<Movie> _movies = new ();
    public Task<bool> CreateAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _movies.SingleOrDefault(x => x.Id == id);
        return Task.FromResult(movie);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        var movie = _movies.SingleOrDefault(x => x.Slug == slug);
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
        if (movieIndex == -1)
        {
            return Task.FromResult(false);
        }

        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removedMovies = _movies.RemoveAll(x => x.Id == id);
        var movieRemoved = removedMovies > 0;
        return Task.FromResult(movieRemoved);
    }

    public Task<bool> ExistedByIdAsync(Guid id)
    {
        return Task.FromResult(_movies.Exists(m => m.Id == id));
    }
}