using MeissnerClockIn.Models;
using MeissnerClockIn.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MeissnerClockIn.Controllers
{
    public class EmployeeController : Controller
    {

        // 
        // GET: /HelloWorld/

        public string Index()
        {
            return "This is my default action...";
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }


        public static void Csv()
        {
            System.Diagnostics.Debug.WriteLine("Start CSV File Reading...\n\n\n");
            var _employeeService = new EmployeeService();
            var path = @"C:\Users\mullarkeyj\source\repos\MeissnerClockIn\WebApplication2\Castlebar Employee List.csv";
            //Here We are calling function to read CSV file  
            var resultData = _employeeService.ReadCSVFile(path);
            //Create an object of the Student class  
            /*Employee employee = new Employee();
            employee.EmployeeId = 5;
            employee.EmployeeLastName = "Mullarkey";
            employee.EmployeeFirstName = "James";
            employee.Status = "In";
            employee.Badge = "777";
            resultData.Add(employee);*/

            DateTime today = DateTime.Today;
            var date = today.ToString();
            string file = Path.Combine(date, "ClockIn.csv");
            string fullPath = Path.Combine(@"C:\Users\mullarkeyj\source\repos\MeissnerClockIn\WebApplication2\", file);
            //Here We are calling function to write file  
            _employeeService.WriteCSVFile(fullPath, resultData);
            //Here D: Drive and Tutorials is the Folder name, and CSV File name will be "NewStudentFile.csv"  
            Console.WriteLine("New File Created Successfully.");
        }
    }
}
