using eCommerce.Data;
using eCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleName.Administrator)]
    [Area("Admin")]
    public class DatabaseController : Controller
    {
        private readonly eCommerceContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseController(eCommerceContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SeedDataAsync()
        {

            var rolename = typeof(RoleName).GetFields().ToList();
            foreach (var r in rolename)
            {
                var roles = (string)r.GetRawConstantValue();
                var rf = await _roleManager.FindByNameAsync(roles);

                if (rf == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roles));
                }
            }

            //create admin user
            var user = await _userManager.FindByNameAsync("admin");
            if (user == null)
            {
                user = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(user, "admin123");
                await _userManager.AddToRoleAsync(user, RoleName.Administrator);

            }

            return RedirectToAction("Index");

        }
    }
}
