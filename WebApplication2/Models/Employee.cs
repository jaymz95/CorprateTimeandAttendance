using CsvHelper;
using CsvHelper.Configuration;
using MeissnerClockIn.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MeissnerClockIn.Models
{

    public sealed class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Map(x => x.EmployeeId).Name("Personnel ID");
            Map(x => x.EmployeeLastName).Name("Last Name");
            Map(x => x.EmployeeFirstName).Name("First Name");
            Map(x => x.Status).Name("Status");
            Map(x => x.Badge).Name("Badges");
        }
    }

    public class Employee
    {
        public int EmployeeId { get; set; }
        //[Display(Name = "Name")]
        public string EmployeeLastName { get; set; }
        public string EmployeeFirstName { get; set; }
        public string Status { get; set; }
        public string Badge { get; set; }





    }
    //public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes);
}
