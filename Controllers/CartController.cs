using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Controllers
{
    [Authorize(Roles = RoleName.Member)]
    public class CartController : Controller
    {
        private readonly eCommerceContext _context;

        public CartController(eCommerceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            var viewModel = new CartVM()
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            return View(viewModel);
        }

        public async Task<ActionResult> AddToCart(int id)
        {
            // Retrieve the album from the database
            var addedProduct = await _context.Products
                                    .Include(p => p.Categories)
                                    .Include(p => p.Tags)
                                    .FirstOrDefaultAsync(p => p.Id == id);
                                

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedProduct);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index", "ViewProduct");
        }

        [HttpPost]
        public async Task<ActionResult> RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            string productName = _context.Carts.Single(c => c.RecordId == id).Product.Name;

            int itemCount = await cart.RemoveFromCart(id);
            var res = new CartRemoveVm()
            {
                Message = productName + " has been removed",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(res);
        }

        

    }
}
