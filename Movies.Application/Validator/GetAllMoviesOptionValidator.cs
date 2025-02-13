using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validator;

public class GetAllMoviesOptionValidator : AbstractValidator<GetAllMoviesOptions>
{
    public GetAllMoviesOptionValidator()
    {
        RuleFor(x => x.Year)
            .LessThanOrEqualTo(DateTime.Now.Year);
        
    }
}