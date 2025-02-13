using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validator;

public class GetAllMoviesOptionValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] SortedFields =
    [
        "Title",
        "YearOfRelease",
    ];
    public GetAllMoviesOptionValidator()
    {
        RuleFor(x => x.Year)
            .LessThanOrEqualTo(DateTime.Now.Year);

        RuleFor(x => x.SortField)
            .Must(x => x is null || SortedFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Invalid sort field");
    }
}