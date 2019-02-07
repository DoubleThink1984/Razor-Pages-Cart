using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesCart.ViewComponents
{
    public class TestViewComponent : ViewComponent
    {
        public TestViewComponent()
        {
            // Inject dependencies
        }

        public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            Dictionary<int, string> myDictionary = new Dictionary<int, string>
            {
                { maxPriority, isDone.ToString() }
            };

            return View(myDictionary);
        }

    }
}
