using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace RazorPagesCart.Areas.Administrator.Pages.Categories
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

        //[BindProperty]
        //public Category Category { get; set; }

        [BindProperty]
        [Display(Name = "Category ID")]
        public int ClassId { get; set; }

        [BindProperty]
        [Display(Name = "Category Name")]
        public string ClassName { get; set; }

        [BindProperty]
        [Display(Name = "Sub-Category ID")]
        public int? ParentClassID { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        [Required]
        public IFormFile MyFormFile { get; set; }

        public IList<SelectListItem> ParentCategories =>
            _db.MenuItems.Select(x => new SelectListItem()
            {
                Text = x.ClassName,
                Value = x.ClassId.ToString()
            }).ToList();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string path = string.Empty;     
            var category = new Category() {
                ClassName = ClassName,
                ParentClassID = ParentClassID,
                Description= Description
            };
            try
            {
                if (MyFormFile != null && MyFormFile.Length > 0)
                {
                    var physicalPath = Path.Combine(_env.WebRootPath, "EntityImages", MyFormFile.FileName);
                    path = "~/MyFiles/EntityImages/" + MyFormFile.FileName;
                    category.ImageUrl = path;
                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        MyFormFile.CopyTo(fileStream);
                    }
                }
                _db.MenuItems.Add(category);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}