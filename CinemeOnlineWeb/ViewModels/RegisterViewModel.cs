using System.ComponentModel.DataAnnotations;

namespace CinemeOnlineWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name="User name")]
        public string UserName { get; set; }  
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name="Birth year")]
        public int Year {get; set;}
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password",ErrorMessage ="Password don't match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
