using Ecommerce.Client.BackendClient;
using Ecommerce.Client.Extensions;
using Ecommerce.Client.Models;
using Ecommerce.Contracts.Endpoints;
using Ecommerce.Contracts.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Client.Pages;

public class IndexModel : PageModel
{
    const int PAGE_SIZE    = 2;
    const int DEFAULT_PAGE = 1;
    private readonly HttpClient client;
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty]
    public string nameFilter { get; set; } = string.Empty;

    [BindProperty]
    public string categoryFilter { get; set; } = string.Empty;

    public PaginatedList<ProductResponse> products { get; set; } = null!;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        client = httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);
    }

    public async Task OnGet()
    {
        var queryString = GetQueryStringForPaginatedProductsRequest(
            DEFAULT_PAGE.ToString(),
            PAGE_SIZE.ToString(),
            nameFilter,
            categoryFilter);

        products = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(
            StoreEndpoints.GetStoreWithProductPaginated,
            queryString: queryString);
    }

    public async Task OnPostAsync()
    {
        var queryString = GetQueryStringForPaginatedProductsRequest(
            DEFAULT_PAGE.ToString(),
            PAGE_SIZE.ToString(),
            nameFilter,
            categoryFilter);

        products = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(
            StoreEndpoints.GetStoreWithProductPaginated,
            queryString: queryString);
    }

    public async Task OnGetChangePageAsync(int pageNumber)
    {
        var queryString = GetQueryStringForPaginatedProductsRequest(
            pageNumber.ToString(),
            PAGE_SIZE.ToString(),
            nameFilter,
            categoryFilter);

        products = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(
            StoreEndpoints.GetStoreWithProductPaginated,
            queryString: queryString);
    }

    private Dictionary<string, string> GetQueryStringForPaginatedProductsRequest(
        string PageNumber,
        string PageSize,
        string nameFilter,
        string categoryFilter)
    {
        return new Dictionary<string, string>()
        {
            {"PageNumber", PageNumber},
            {"PageSize", PageSize},
            {"NameFilter", nameFilter},
            {"CategoryFilter", categoryFilter}
        };
    }
}