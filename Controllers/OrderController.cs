using AutoMapper;
using BussinessLayer.Repos;
using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentaion.Controllers
{
    public class OrderController : Controller
    {
        private readonly ShopingCartRepo _cartRepo;
        private readonly IMapper mapper;
        private readonly OrderRepo _orderRepo;

        public OrderController(ShopingCartRepo cartRepo, IMapper mapper, OrderRepo orderRepo)
        {
            this._cartRepo = cartRepo;
            this.mapper = mapper;
            this._orderRepo = orderRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> MakeOrder(float totalamount)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartRepo.GetById(userid);
            await _orderRepo.MakeOrder(cart, totalamount);
            return RedirectToAction("ShowOrders");
        }
        public async Task<IActionResult> ShowOrders()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var x = await _orderRepo.GetAllAsync(userid);
            return View("Orders", x);
        }
        public async Task<IActionResult>OrderDeatils(string id)
        {
           var x=await _orderRepo.GetByIdAsync(id);
            return View(x);
        }

    }
}
