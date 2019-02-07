using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPagesCart.Pages
{
    public class AboutModel : PageModel
    {
        private ILogger<AboutModel> _logger;

        public AboutModel(ILogger<AboutModel> logger)
        {
            this._logger = logger;
        }

        public string Message { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("This is logging here");
            Message = "Your application description page.";
            ViewData["SessionString"] = HttpContext.Session.GetString("Test");
        }
    }
}
