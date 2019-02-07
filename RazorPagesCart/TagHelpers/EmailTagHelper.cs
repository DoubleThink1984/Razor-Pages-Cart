using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RazorPagesCart.TagHelpers
{
    public class EmailTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //base.Process(context, output);
            output.TagName = "div";
            output.Attributes.Add("class", "btn btn-default");
            foreach (var item in context.Items)
            {
                output.Content.Append(item.ToString());
            }
            foreach (var item in context.AllAttributes)
            {
                output.Content.Append(item.Name + " : " + item.Value);
            }
            
            if (output.Attributes.TryGetAttribute("is-it-here", out TagHelperAttribute attribute))
            {
                output.Content.Append(attribute.Value.ToString());
            }
        }
    }
}
