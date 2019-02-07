using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;
using RazorPagesCart.Infrastructure;

namespace RazorPagesCart.Areas.Administrator.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _db;
        public List<Category> Categories { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            this._db = db;
        }

        public void OnGet()
        {
            //Categories = HierarchicalModelLists.Categories;
            Categories = _db.MenuItems.OrderBy(x => x.ClassName).ToList();
        }
    }
}