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
    const int PAGE_SIZE = 2;
    const int DEFAULT_PAGE = 1;

    private readonly IHttpClientFactory _httpClientFactory;

    public string productToSearch { get; set; } = string.Empty;

    public PaginatedList<ProductResponse> storeProducts { get; set; } = null!;

    public IndexModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task OnGet()
    {
        var client = _httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);
        
        var query = GetQueryForPaginatedProductsRequest(DEFAULT_PAGE.ToString(), PAGE_SIZE.ToString());

        storeProducts = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(StoreEndpoints.GetStoreWithProductPaginated, query: query);
    }

    public async Task<IActionResult> OnGetNextPageAsync(int pageNumber)
    {
        var client = _httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);

        var query = GetQueryForPaginatedProductsRequest(pageNumber.ToString(), PAGE_SIZE.ToString());

        storeProducts = await client.GetAsyncWrapper<PaginatedList<ProductResponse>>(StoreEndpoints.GetStoreWithProductPaginated, query: query);

        return Page();
    }

    private Dictionary<string, string> GetQueryForPaginatedProductsRequest(string PageNumber, string PageSize)
    {
        return new Dictionary<string, string>()
        {
            {"PageNumber", PageNumber},
            {"PageSize", PageSize}
        };
    }
}