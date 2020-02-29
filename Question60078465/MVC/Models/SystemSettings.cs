using System.ComponentModel.DataAnnotations;

namespace Question60078465.MVC.Models
{
    public class SystemSettings
    {
        [Display(Name = "Email From", Description = "This is the email from address")]
        public string EmailFromAddress { get; set; }
    }
}