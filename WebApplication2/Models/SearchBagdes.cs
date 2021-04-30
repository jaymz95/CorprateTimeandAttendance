using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeissnerClockIn.Models
{
    public class SearchBadge
    {
        public string Badge { get; set; }

    }

    namespace HTMLHelpersDemo.Helpers
    {
        public static class MyHTMLHelpers
        {
            public static HtmlString LabelWithMark(string content)
            {
                string htmlString = String.Format("<label><mark>{0}</mark></label>", content);
                return new HtmlString(htmlString);
            }
        }
    }
}
