using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;

namespace Movies.Application;

public static class ApplicationServicesCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
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