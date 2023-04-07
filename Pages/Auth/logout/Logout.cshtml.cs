using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace Ecommerce.Client.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public void OnGet(){}

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            RemoveTokenCookies();
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Home/Index");
        }

        private void RemoveTokenCookies()
        {
            Response.Cookies.Delete("access-token");
            Response.Cookies.Delete("refresh-token");
        }
    }
}