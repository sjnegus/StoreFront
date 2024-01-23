using Microsoft.AspNetCore.Mvc;
using StoreFront.DATA.EF.Models;
using Microsoft.AspNetCore.Identity;
using StoreFront.UI.MVC.Models;
using Newtonsoft.Json;

namespace StoreFront.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly StoreFrontContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartController(StoreFrontContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var localCart = GetCart();
            if (!localCart.Any())
            {
                ViewBag.Message = "There are no items in your cart.";
            }
            return View(localCart);
        }

        public IActionResult AddToCart(int id) 
        {
            var localCart = GetCart();
            Product? p = _context.Products.Find(id);
            if (p == null)
            {
                ViewBag.Message = "Invalid Product ID";
                return RedirectToAction(nameof(Index));
            }

            if (localCart.ContainsKey(p.ProductId))
            {
                localCart[p.ProductId].Qty++;
            }
            else
            {
                localCart.Add(p.ProductId, new(1, p));
            }
            SetCart(localCart);
            return RedirectToAction(nameof(Index));
        }


        private Dictionary<int, CartItemViewModel> GetCart()
        {
            var jsonCart = HttpContext.Session.GetString("cart");
            if (string.IsNullOrEmpty(jsonCart))
            {
                return new();
            }
            return JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(jsonCart);
        }

        private void SetCart(Dictionary<int, CartItemViewModel> localCart)
        {
            if (localCart == null || !localCart.Any()) 
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                var jsonCart = JsonConvert.SerializeObject(localCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }
        }






    }
}
