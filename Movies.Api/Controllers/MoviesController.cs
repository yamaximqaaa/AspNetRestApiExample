using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController]
public class MoviesController(IMovieRepository movieRepository) : ControllerBase
{
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<ActionResult> Create([FromBody]CreateMovieRequest request)
    {
        var movie = request.MapToMovie();
        await movieRepository.CreateAsync(movie);
        
        //return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie);               // do same things, but second
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);   // way better 
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieRepository.GetByIdAsync(id)
            : await movieRepository.GetBySlugAsync(idOrSlug);
        if (movie is null)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await movieRepository.GetAllAsync();

        var moviesResponse = movies.MapToResponse();
        return Ok(moviesResponse);
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute]Guid id,
        [FromBody]UpdateMovieRequest updateMovieRequest)
    {
        var movie = updateMovieRequest.MapToMovie(id);

        var update = await movieRepository.UpdateAsync(movie);
        if (!update)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await movieRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}