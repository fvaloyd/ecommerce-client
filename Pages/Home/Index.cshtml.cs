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

    [BindProperty]
    public string nameFilter     { get; set; } = string.Empty;
    [BindProperty]
    public string categoryFilter { get; set; } = string.Empty;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaginatedList<ProductResponse> storeProducts { get; set; } = null!;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        client = httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);
    }

    public async Task OnGet()
    {
        // var client = _httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);
        
        var query = GetQueryForPaginatedProductsRequest(DEFAULT_PAGE.ToString(), PAGE_SIZE.ToString(), nameFilter, categoryFilter);

        storeProducts = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(StoreEndpoints.GetStoreWithProductPaginated, query: query);
    }

    public async Task OnPostAsync()
    {
        // var client = _httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);
        
        var query = GetQueryForPaginatedProductsRequest(DEFAULT_PAGE.ToString(), PAGE_SIZE.ToString(), nameFilter, categoryFilter);

        storeProducts = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(StoreEndpoints.GetStoreWithProductPaginated, query: query);
    }

    public async Task OnGetNextPageAsync(int pageNumber)
    {
        // var client = _httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);

        var query = GetQueryForPaginatedProductsRequest(pageNumber.ToString(), PAGE_SIZE.ToString(), nameFilter, categoryFilter);

        storeProducts = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(StoreEndpoints.GetStoreWithProductPaginated, query: query);
    }

    private Dictionary<string, string> GetQueryForPaginatedProductsRequest(
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