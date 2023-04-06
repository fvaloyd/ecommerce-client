using Ecommerce.Contracts.Responses;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Client.Pages.Home.Product;

public class ProductDetail : PageModel
{
    public ProductResponse product = null!;
    
    public void OnGet()
    {
    }
}