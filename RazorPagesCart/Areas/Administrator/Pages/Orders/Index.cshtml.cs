using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;

namespace RazorPagesCart.Areas.Administrator.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            this._db = db;
        }

        public List<Order> orders;

        public async Task OnGetAsync(string somethingElse)
        {
            if (!string.IsNullOrWhiteSpace(somethingElse))
            {
                ViewData["SomeData"] = somethingElse;
            }
            orders = await _db.Orders.OrderBy(x => x.OrderID).ToListAsync();
        }
    }
}