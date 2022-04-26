using Microsoft.AspNetCore.Identity;

namespace CinemeOnlineWeb.Models
{
    public class User:IdentityUser
    {
        public int Year { get; set; }
    }
}
