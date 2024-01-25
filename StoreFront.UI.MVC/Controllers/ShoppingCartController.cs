using Microsoft.AspNetCore.Mvc;
using StoreFront.DATA.EF.Models;
using Microsoft.AspNetCore.Identity;
using StoreFront.UI.MVC.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            var localCart = GetCart();
            if (localCart.ContainsKey(id))
            {
                localCart.Remove(id);
            }
            SetCart(localCart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateCart(int id, int qty)
        {
            var localCart = GetCart();
            if (localCart.ContainsKey(id))
            {
                RemoveFromCart(id);
            }
            if (localCart.ContainsKey(id))
            {
                localCart[id].Qty = qty;
            }
            SetCart(localCart);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            if (!GetCart().Any())
            {
                return RedirectToAction("Index");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ud = await _context.UserDetails.FindAsync(userId);
            if (ud == null)
            {
                ud = new() { UserId = userId };
                _context.Add(ud);
                await _context.SaveChangesAsync();
            }
            CheckoutViewModel model = new()
            {
                ShipToName = (ud?.FirstName + " " + ud?.LastName),
                ShipCity = ud?.City,
                ShipState = ud?.State,
                ShipZip = ud?.Zip
            };
            return await Checkout(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Create the Order Object and assign values
            Order o = new()
            {
                OrderDate = DateTime.Now,
                UserId = userId,

            };
            var localCart = GetCart();
            _context.Orders.Add(o); // add our order to the context before populating the OrderProducts so that we have an OrderId to use
            foreach (var item in localCart.Values)
            {
                // for every CartItemViewModel in the Dictionary Cart, create an OrderProduct object
                ProductOrder po = new()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Product.ProductId,
                    Price = (decimal)item.Product.Price,
                    Quantity = (short)item.Qty
                };
                // Add each of those OrderProducts to the collection property of the Order
                o.ProductOrders.Add(po);
            }
            // Commit those changes to the DB
            await _context.SaveChangesAsync();

            SetCart(new()); // send an empty cart back to the session

            return RedirectToAction(nameof(Index), "Orders");
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
