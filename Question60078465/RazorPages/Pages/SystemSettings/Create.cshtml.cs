using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Question60078465.RazorPages.Pages.SystemSettings
{
    public class CreateModel : PageModel
    {
        public Models.SystemSettings SystemSettings { get; set; }

        public void OnGet() { }
    }
}