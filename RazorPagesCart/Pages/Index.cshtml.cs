using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;
using RazorPagesCart.Hubs;

namespace RazorPagesCart.Pages
{
    public class IndexModel : PageModel
    {
        #region Private Fields
        private ApplicationDbContext _db;
        private IHubContext<ChatHub> _hubContext;
        #endregion

        #region Constructors
        public IndexModel(ApplicationDbContext db, IHubContext<ChatHub> hubContext)
        {
            this._db = db;
            this._hubContext = hubContext;
        }
        #endregion

        #region Public Properties
        public List<Product> FeaturedProducts { get; set; } 
        #endregion

        #region HTTP Methods
        public async Task OnGetAsync()
        {
            HttpContext.Session.SetString("Test", "I suppose it worked!");

            await _hubContext.Clients.All.SendAsync("ReceivedMessageFromServerPage", "Sent from Index Page OnGet");

            FeaturedProducts = _db.Products.Include(x => x.ProductImages).OrderBy(x => x.ProductName).ToList();
        } 
        #endregion
    }
}
