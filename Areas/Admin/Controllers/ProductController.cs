using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eCommerce.Data;
using eCommerce.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace eCommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleName.Administrator + "," + RoleName.Editor)]
    public class ProductController : Controller
    {
        private readonly eCommerceContext _context;

        public ProductController(eCommerceContext context)
        {
            _context = context;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index([FromQuery(Name ="p")] int currentPage, int pagesize)
        {
            var products = _context.Products.Include(p => p.Categories).Include(p => p.Tags);

            int totalProduct = await products.CountAsync();

            if (pagesize <= 0) pagesize = 10;
            int countPages = (int)Math.Ceiling((double)totalProduct / pagesize);


            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage= 1;

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

        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TagsId"] = new SelectList(_context.Tags, "Id", "Name");
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.Name) + extension;
                    product.Thumbnail = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
            ViewData["TagsId"] = new SelectList(_context.Tags, "Id", "Name", product.TagsId);
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoriesId);
            ViewData["TagsId"] = new SelectList(_context.Tags, "Id", "Name", product.TagsId);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.Name) + extension;
                        product.Thumbnail = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriesId"] = new SelectList(_context.Categories, "Id", "Image", product.CategoriesId);
            ViewData["TagsId"] = new SelectList(_context.Tags, "Id", "Name", product.TagsId);
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
