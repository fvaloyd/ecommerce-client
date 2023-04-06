namespace Ecommerce.Client.Models;
public class PaginatedList<T>
{
    public PaginatedList() { }

    public IList<T> Items { get; set; } = null!;
    public int TotalCount {get; set;}
    public int PageNumber {get; set;}
    public int TotalPages {get; set;}

    public PaginatedList(IList<T> items, int count, int pageSize, int pageNumber)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}