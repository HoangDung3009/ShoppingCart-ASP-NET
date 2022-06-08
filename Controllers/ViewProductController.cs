using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Controllers
{
    
    public class ViewProductController : Controller
    {

        private readonly eCommerceContext _context;
        

        public ViewProductController(eCommerceContext context)
        {
            _context = context;
        }

        [Route("product/index.html")]
        public async Task<IActionResult> Index([FromQuery(Name = "p")] int currentPage, int pagesize)
        {
            var products = _context.Products.Include(p => p.Categories).Include(p => p.Tags);

            int totalProduct = await products.CountAsync();

            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalProduct / pagesize);


            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

            var pagingmodel = new PagingModel()
            {
                countpages = countPages,
                currentpage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pagesize = pagesize
                })

            };

            ViewBag.PagingModel = pagingmodel;
            ViewBag.totalProduct = totalProduct;

            var productInPage = await products.Skip((currentPage - 1) * pagesize)
                                        .Take(pagesize)
                                        .Include(p => p.Categories)
                                        .Include(p => p.Tags)
                                        .ToListAsync();


            return View(productInPage);
        }

        [Route("product/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                                .Include(p => p.Categories)
                                .Include(p => p.Tags)
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); 
            }

            return View(product);
        }


    }


}
