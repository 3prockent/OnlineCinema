using CinemeOnlineWeb.Models;
using CinemeOnlineWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace CinemeOnlineWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Year < 1922 || model.Year > DateTime.Now.Year)
                ModelState.AddModelError("Year", "Invalid year");
            if (ModelState.IsValid)
            {
                var user = new User { Email = model.Email,UserName=model.UserName, Year = model.Year};
                var result = await _userManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await _userManager.AddToRoleAsync(user, "user");
                    
                    return RedirectToAction("Index", "Films");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError("Password", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl=null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // перевіряємо, чи належить URL додатку
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Films");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Password or UserName");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // видаляємо аутентифікаційні куки
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Films");
        }


    }

}
