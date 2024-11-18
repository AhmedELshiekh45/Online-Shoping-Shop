using BussinessLayer.Repos;
using BussinessLayer.ViewModels;
using DataAccess;
using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;
using Services_Layer.Services;
using Stripe;
using System.Security.Claims;

namespace Presentaion.Controllers
{
    public class CardController : Controller
    {
        private readonly ShopingCartRepo _cartRepo;
        private readonly IConfiguration _configuration;

        public CardController(ShopingCartRepo cartRepo,IConfiguration configuration)
        {
            this._cartRepo = cartRepo;
            this._configuration = configuration;
        }

        public async Task<IActionResult> AddToCard(string id, int Quantity = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _cartRepo.AddToCard(userid, id, Quantity);

                return RedirectToAction("ShowAll", "Product");
            }
            return RedirectToAction("LogIn", "Account");
        }
        public async Task<IActionResult> Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var vm = await _cartRepo.GetCardInfoAsync(userid);


                return View(vm);
            }
            return RedirectToAction("LogIn", "Account");
        }
        public async Task<IActionResult> Remove(string id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartRepo.RemoveFromCard(userid, id);
            return RedirectToAction("Profile");
        }

    
        


    }
}
