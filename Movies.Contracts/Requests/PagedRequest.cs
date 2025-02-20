namespace Movies.Contracts.Requests;

public class PagedRequest
{
    public required int Page { get; init; } = 1;
    
    public required int PageSize { get; init; } = 10;
}

// -- If needed more than one. Implement required and default values in the main contract

// public interface PagedRequest
// {
//     public int Page { get; init; }
//     
//     public int PageSize { get; init; }
// }