using Ecommerce.Client.BackendClient;
using Ecommerce.Client.Extensions;
using Ecommerce.Client.Models;
using Ecommerce.Contracts.Categories;
using Ecommerce.Contracts.Endpoints;
using Ecommerce.Contracts.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Client.Pages;

public class IndexModel : PageModel
{
    const string CACHE_CATEGORIES_KEY = "categories";
    const int PAGE_SIZE    = 2;
    const int DEFAULT_PAGE = 1;
    private readonly HttpClient client;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;

    [BindProperty]
    public string nameFilter { get; set; } = string.Empty;

    [BindProperty]
    public string categoryFilter { get; set; } = string.Empty;

    public PaginatedList<ProductResponse> products { get; set; } = null!;

    public string[] categories {get;set;} = null!;

    public IndexModel(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        client = httpClientFactory.CreateClient(BackendClientConsts.CLIENT_NAME);
        _cache = cache;
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

        categories = await GetCacheCategory();
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

        categories = await GetCacheCategory();
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

        categories = await GetCacheCategory();
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

    private async Task<string[]> GetCacheCategory()
    {
        if (!_cache.TryGetValue(CACHE_CATEGORIES_KEY, out string[]? cacheCategories))
        {
            cacheCategories = (await client.GetAsyncWrapper<ICollection<CategoryResponse>>(CategoryEndpoints.GetAllCategories))
                            .Select(cr => cr.Name)
                            .ToArray();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromDays(1));

            _cache.Set(CACHE_CATEGORIES_KEY, cacheCategories, cacheEntryOptions);
        }

        return cacheCategories!;
    }
}