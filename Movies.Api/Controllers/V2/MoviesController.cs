using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers.V2;

[ApiController]
[ApiVersion(2.0)]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPost(ApiEndpoints.V2.Movies.Create)]
    public async Task<ActionResult> Create([FromBody]CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var movie = request.MapToMovie();
        await movieService.CreateAsync(movie, cancellationToken);
        
        //return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie);               // do same things, but second
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);   // way better 
    }
    
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.V2.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieService.GetByIdAsync(id, userId, cancellationToken)
            : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);
        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        
        /*  -- Example of usage HATEOAS --  
        
        // 1. add this to method params: [FromServices] LinkGenerator linkGenerator,
        // 2. update MovieResponse class to -> public class MovieResponse : HalResponse
        
        var movieObj = new { id = movie.Id };
        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new {idOrSlug = movie.Id}),
            Rel = "self",
            Type = "GET"
        });
        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: movieObj),
            Rel = "self",
            Type = "PUT"
        });
        response.Links.Add(new Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: new {id = movie.Id}),
            Rel = "self",
            Type = "DELETE"
        });
        */
        
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.V2.Movies.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest optionRequest, 
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var filerOptions = optionRequest.MapToOptions()
            .WithUser(userId);

        var moviesCount = await movieService.GetCountAsync(filerOptions.Title, filerOptions.Year, cancellationToken);
        
        var movies = await movieService.GetAllAsync(filerOptions, cancellationToken);

        var moviesResponse = movies.MapToResponse(optionRequest.Page, optionRequest.PageSize, moviesCount);
        return Ok(moviesResponse);
    }
    
    [Authorize(AuthConstants.TrustedUserPolicyName)]
    [HttpPut(ApiEndpoints.V2.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id,
        [FromBody]UpdateMovieRequest updateMovieRequest,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();
        var movie = updateMovieRequest.MapToMovie(id);

        var updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);
        if (updatedMovie is null)
        {
            return NotFound();
        }

        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.V2.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await movieService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}