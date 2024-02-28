using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application;

public static class ApplicationServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepositoryDb>();
        services.AddSingleton<IMovieService, MovieService>();
        return services;
    }
    public static IServiceCollection AddDatabase(this IServiceCollection services, 
        string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => 
            new NpgSqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}