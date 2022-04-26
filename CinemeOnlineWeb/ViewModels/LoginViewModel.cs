using System.ComponentModel.DataAnnotations;

namespace CinemeOnlineWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }
}
