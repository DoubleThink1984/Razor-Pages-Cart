using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;

namespace RazorPagesCart.Areas.Administrator.Pages.Products
{
    public class EditModel : PageModel
    {
        private ApplicationDbContext _db;
        private IHostingEnvironment _env;

        public EditModel(ApplicationDbContext db, IHostingEnvironment env)
        {
            this._db = db;
            this._env = env;
        }

        public List<SelectListItem> Categories =>
            _db.MenuItems.Select(x => new SelectListItem
            {
                Value = x.ClassId.ToString(),
                Text = x.ClassName
            }).ToList();

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public IFormFile MyFormFile { get; set; }

        public void OnGet(int id)
        {
            Product = _db.Products.Include(x => x.ProductImages).Where(x => x.ProductID == id).FirstOrDefault();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("/Index");
        }
    }
}