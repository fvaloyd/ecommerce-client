namespace Ecommerce.Client.Models;
public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items {get;} = null!;
    public int TotalCount {get; set;}
    public int PageNumber {get; set;}
    public int TotalPages {get; set;}

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageSize, int pageNumber)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}