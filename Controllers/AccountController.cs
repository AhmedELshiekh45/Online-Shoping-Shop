using DataAccess.Constaats;
using DataAccess.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Repos;
using ServiceLayer.ViewModels;

namespace Presentaion.Controllers
{
    [Authorize(Roles = "Admin,Customer")]
    public class AccountController : Controller
    {
        private readonly UserRepo _UserServices;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(UserRepo userservice, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this._UserServices = userservice;
            this._signInManager = signInManager;
            this._userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _UserServices.CheckLogIn(loginVM);

                if (result)
                {
                    var user = await _userManager.FindByNameAsync(loginVM.UserName);
                    await _signInManager.SignInAsync(user, loginVM.RememberMe);
                    
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "User Name Or Password Are Wrong");
            }
            return View(loginVM);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _UserServices.Register(registerVM);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(registerVM.UserName), Roles.Customer.ToString());
                    return RedirectToAction("login");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(registerVM);
        }

        public async Task<IActionResult> LogOut ()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction("LogIn");
        }
    }
}
