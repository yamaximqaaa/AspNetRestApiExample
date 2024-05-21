using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;

namespace Movies.Application;

public static class ApplicationServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IRatingRepository, RatingRepository>();
        services.AddSingleton<IMovieRepository, MovieRepositoryDb>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);    // TODO: how this works?
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