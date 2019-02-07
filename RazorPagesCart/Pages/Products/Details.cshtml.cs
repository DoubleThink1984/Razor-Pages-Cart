using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;

namespace RazorPagesCart.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Product Product { get; set; }

        //[BindProperty(SupportsGet = true)]
        //public int Id { get; set; }

        public void OnGet(int id)
        {
            Product = _db.Products.Include(x => x.ProductImages).Where(x => x.ProductID == id).FirstOrDefault();
        }
    }
}