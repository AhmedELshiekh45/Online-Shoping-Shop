using AutoMapper;
using BussinessLayer.Repos;
using BussinessLayer.ViewModels;
using DataAccess;
using DataAccess.Model;
using Microsoft.AspNetCore.Mvc;
using Services_Layer.Services;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Presentaion.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly ShopingCartRepo _cartRepo;
        private readonly IMapper mapper;
        private readonly OrderRepo _orderRepo;

        public CheckOutController(ShopingCartRepo cartRepo, IMapper mapper, OrderRepo orderRepo)
        {
            this._cartRepo = cartRepo;
            this.mapper = mapper;
            this._orderRepo = orderRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        dynamic totalAmount = 0;

        private async Task<float> CalculateTotalAmount()
        {      // This would typically pull data from your database to show products in the cart.
            // For simplicity, we're hardcoding a product.
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _cartRepo.GetCartItemsAsync(userid);

            foreach (var item in items)
            {

                totalAmount += item.Quantity * item.Product.UnitPrice;
            }
            return totalAmount;
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            // This would typically pull data from your database to show products in the cart.
            // For simplicity, we're hardcoding a product.
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = await _cartRepo.GetCartItemsAsync(userid);

            // Calculate the total price

            var list = mapper.Map<HashSet<CartItemVm>>(items);
            return View(new CheckOutVM
            {
                CartItems = list,
                TotalAmount = await CalculateTotalAmount()
            });
        }

        // [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartRepo.GetCartItemsAsync(userid);
            // Convert cart items to Stripe's line items
            var lineItems = new List<SessionLineItemOptions>();
            foreach (var item in cartItems)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                        UnitAmountDecimal = (decimal)item.Product.UnitPrice * 100, // Stripe expects amount in cents
                    },
                    Quantity = item.Quantity,
                });
            }
            var x = await CalculateTotalAmount();
            // Create a Checkout Session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment", // Use 'payment' mode for one-time purchases
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Order/MakeOrder?totalamount={ x}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // Redirect to Stripe Checkout page
            return Redirect(session.Url);
        }

       

    }
}
