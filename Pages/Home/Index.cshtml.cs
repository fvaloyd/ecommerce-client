using Ecommerce.Client.Models;
using Ecommerce.Client.Services;
using Ecommerce.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Client.Pages;

public class IndexModel : PageModel
{
    const string CACHE_CATEGORIES_KEY = "Categories";
    const int PAGE_SIZE    = 6;
    const int DEFAULT_PAGE = 1;

    private readonly IMemoryCache _cache;
    private readonly IEcommerceApi _ecommerceApi;

    public IndexModel(IMemoryCache cache, IEcommerceApi ecommerceApi)
        => (_cache, _ecommerceApi) = (cache, ecommerceApi);

    [BindProperty]
    public string NameFilter { get; set; } = string.Empty;

    [BindProperty]
    public string CategoryFilter { get; set; } = string.Empty;

    public PaginatedList<ProductResponse> Products { get; set; } = new();

    public string[] Categories { get; set; } = Array.Empty<string>();

    public int[] ProductInCartIds { get; set; } = Array.Empty<int>();

    public async Task OnGet()
    {
        await LoadPageData(DEFAULT_PAGE);
    }

    public async Task OnPostAsync()
    {
        await LoadPageData(DEFAULT_PAGE);
    }

    public async Task OnGetChangePageAsync(int pageNumber)
    {
        await LoadPageData(pageNumber);
    }

    private async Task LoadPageData(int pageNumber)
    {
        Products = await _ecommerceApi.GetAllProductsOnStore(pageNumber, PAGE_SIZE, NameFilter, CategoryFilter);

        Categories = await GetCacheCategory(OnCacheMiss);

        await LoadProductIdsFromCart();
    }

    private async Task<string[]> OnCacheMiss()
        => (await _ecommerceApi.GetAllCategories()).Select(c => c.Name).ToArray();

    private async Task<string[]> GetCacheCategory(Func<Task<string[]>> onCacheMiss)
    {
        if (!_cache.TryGetValue(CACHE_CATEGORIES_KEY, out string[]? categories))
        {
            categories = await onCacheMiss();

            CacheCategories(categories);
        }

        return categories!;
    }

    private void CacheCategories(string[] categories)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromDays(1));

        _cache.Set(CACHE_CATEGORIES_KEY, categories, cacheEntryOptions);
    }

    private async Task LoadProductIdsFromCart()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
        {
            // The fact that the user can have an empty cart forces us to ignore this ApiException.
            try { ProductInCartIds = await _ecommerceApi.GetProductIdsInBasket(); }
            catch { }
        }
    }

    public async Task<IActionResult> OnGetAddToCartAsync(int productId)
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToPage("/Auth/Login/Login");

        await _ecommerceApi.AddProductToBasket(productId);
        return RedirectToPage("/Home/Index");
    }
}