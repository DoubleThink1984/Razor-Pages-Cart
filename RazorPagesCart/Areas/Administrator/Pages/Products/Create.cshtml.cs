using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;

namespace RazorPagesCart.Areas.Administrator.Pages.Products
{
    public class CreateModel : PageModel
    {
        private ApplicationDbContext _db;
        private IHostingEnvironment _env;

        public CreateModel(ApplicationDbContext db, IHostingEnvironment env)
        {
            this._db = db;
            this._env = env;
        }

        public List<SelectListItem> Categories =>
            _db.MenuItems.Select(x => new SelectListItem {
                Value = x.ClassId.ToString(),
                Text = x.ClassName
            }).ToList();

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public IFormFile MyFormFile { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost([FromServices]ApplicationDbContext database)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string path = string.Empty;
            try
            {
                database.Products.Add(Product);

                if (MyFormFile != null && MyFormFile.Length > 0)
                {
                    //var asdf = Url.Content("~/MyFiles/EntityImages" );
                    var physicalPath = Path.Combine(_env.WebRootPath, "EntityImages", MyFormFile.FileName);
                    path = "~/MyFiles/EntityImages/" + MyFormFile.FileName;
                    var image = new ProductImages()
                    {
                        Featured = true,
                        Path = path,
                        ProductID = Product.ProductID
                    };
                    database.ProductImages.Add(image);

                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        MyFormFile.CopyTo(fileStream);
                    }
                }
                _db.SaveChanges();

                return RedirectToPage("./Index");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}