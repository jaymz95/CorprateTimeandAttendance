using CsvHelper;
using MeissnerClockIn.Models;
using MeissnerClockIn.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MeissnerClockIn.Controllers
{
    //private bool done;
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        List<string> badges = new List<string>();
        List<string> empName = new List<string>();
        List<string> lastClock = new List<string>();
        List<string> inOutStatus = new List<string>();
        bool wait = false;
        bool totaled = false;
        private Boolean clear = false;
        public Boolean BoolValue
        {
            get { return clear; }
            set
            {
                clear = value;
                // trigger event (you could even compare the new value to
                // the old one and trigger it when the value really changed)
                ViewBag.time = "";
            }
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Example()
        {
            var model = new Employee();
            return View(model);
        }

        public IActionResult Index(SearchBadge vm)
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            var yesturdayDate = yesterday.ToString(@"ddMMMMyyyy");
            string yesterdayFile = yesturdayDate + "ClockIn.csv";
            string yesterdayCsv = Path.Combine(Environment.CurrentDirectory, @"TimeSheets", yesterdayFile);

            System.Diagnostics.Debug.WriteLine("\nNOsdgfTERDAY##################################### " + yesterdayCsv);
            if (System.IO.File.Exists(yesterdayCsv))
            {
                TotalService ts = new TotalService();
                ts.Total();

                System.Diagnostics.Debug.WriteLine("\nYESsdgfTERDAY##################################### ");
            }
            DateTime today = DateTime.Today;
            var date = today.ToString(@"ddMMMMyyyy");
            string file = date + "ClockIn.csv";
            var _employeeService = new EmployeeService();
            string fullPath = Path.Combine(Environment.CurrentDirectory, @"TimeSheets", file);
            var badgePath = Path.Combine(Environment.CurrentDirectory, @"Badges.csv");
            var readPath = Path.Combine(Environment.CurrentDirectory, @"Castlebar Employee List.csv");

            string clockIn = Path.Combine(Environment.CurrentDirectory, @"ClockIn.csv");
            ViewBag.readPath = readPath;
            ViewBag.clockIn = clockIn;
            var employeeList = _employeeService.ReadCSVFile(readPath);
            //bool newFile;

            try
            {
                //System.Diagnostics.Debug.WriteLine("badge path.................... " + badgePath);
                string[] records, records2, records3, records4;
                int list = 0;
                var total = employeeList.Count;
                if (System.IO.File.Exists(fullPath))
                {
                    using (var reader = new StreamReader(badgePath, Encoding.Default))
                    {
                        records = reader.ReadLine().Split(",");
                        records2 = reader.ReadLine().Split(",");
                        records3 = reader.ReadLine().Split(",");
                        records4 = reader.ReadLine().Split(",");
                    }
                    while (list < total)
                    {
                        //System.Diagnostics.Debug.WriteLine("\n\t\tindex\t\t" + list);
                        empName.Add(records[list]);
                        badges.Add(records2[list]);
                        lastClock.Add(records3[list]);
                        inOutStatus.Add(records4[list]);
                        list++;
                    }
                }

                EmployeeService employeeService = new EmployeeService();
                //Here We are calling function to read CSV file
                var resultData = employeeService.ReadCSVFile(readPath);

                if (!System.IO.File.Exists(fullPath))
                {

                    var count = 0;
                    var empCount = employeeList.Count();

                    List<string> badgeName = new List<string>();
                    List<string> badgeNumber = new List<string>();
                    List<string> badgeTime = new List<string>();
                    List<string> badgeInOut = new List<string>();

                    // Creating Employee Clockin CSV file
                    while (count < empCount)
                    {
                        // Adding column names with employee names
                        badgeName.Add(employeeList.Select(c => c.EmployeeFirstName).ToList().ElementAt(count) + " " + employeeList.Select(c => c.EmployeeLastName).ToList().ElementAt(count));

                        //System.Diagnostics.Debug.WriteLine("\n\t\tname\n" + badgeName[count] + "\n");
                        count++;
                    }
                    count = 0;
                    while (count < empCount)
                    {
                        // Adding column names with employee ID's
                        badgeNumber.Add(employeeList.Select(c => c.Badge).ToList().ElementAt(count));
                        badgeTime.Add("");
                        badgeInOut.Add("");
                        //System.Diagnostics.Debug.WriteLine("\n\t\tnum\n" + badgeNumber[count] + " " + badgeTime[count] + " " + badgeInOut[count] + "\n");
                        count++;
                    }
                    records = badgeName.ToArray();
                    records2 = badgeNumber.ToArray();
                    records3 = badgeTime.ToArray();
                    records4 = badgeInOut.ToArray();

                    list = 0;
                    total = employeeList.Count;
                    while (list < total)
                    {
                        empName.Add(records[list]);
                        badges.Add(records2[list]);
                        lastClock.Add(records3[list]);
                        inOutStatus.Add(records4[list]);
                        list++;
                    }

                    //Here We are calling function to read CSV file  
                    //newFile = true;
                    int j = 0;
                    while (j < lastClock.Count)
                    {
                        // Adding column names with employee ID's
                        lastClock[j] = "";
                        inOutStatus[j] = "";
                        j++;
                    }

                    using (StreamWriter sw = new StreamWriter(fullPath, false, new UTF8Encoding(true)))
                    using (CsvWriter cw = new CsvWriter(sw, CultureInfo.InvariantCulture))
                    {
                        int i = 0;
                        empCount = employeeList.Count();

                        // Creating Employee Clockin CSV file
                        while (i < empCount)
                        {
                            // Adding column names with employee names
                            cw.WriteField(employeeList.Select(c => c.EmployeeFirstName).ToList().ElementAt(i) + " " + employeeList.Select(c => c.EmployeeLastName).ToList().ElementAt(i));

                            i++;
                        }
                        cw.NextRecord();
                        i = 0;
                        while (i < empCount)
                        {
                            // Adding column names with employee ID's
                            cw.WriteField(employeeList.Select(c => c.EmployeeId).ToList().ElementAt(i));
                            i++;
                        }

                    }
                }
                else
                {
                    //newFile = false;
                }

                var scannedEmployeeInd = -1;
                // Outputting Employee Details when Scanned
                // and Time of Clock In/Out
                var empBadge = vm.Badge;
                string inOut = "In";
                if (empBadge != null && empBadge.Length > 4)
                {
                    Employee scannedEmployee = resultData.Find(x => x.Badge.Contains(empBadge));
                    if (scannedEmployee == null)
                    {
                        ViewBag.time = "This user does not exist yet";
                    }
                    else
                    {
                        var scannedEmpName = scannedEmployee.EmployeeFirstName + " " + scannedEmployee.EmployeeLastName;

                        Employee employee = new Employee();
                        employee.EmployeeId = scannedEmployee.EmployeeId;
                        employee.EmployeeLastName = scannedEmployee.EmployeeLastName;
                        employee.EmployeeFirstName = scannedEmployee.EmployeeFirstName;
                        employee.Status = scannedEmployee.Status;
                        employee.Badge = scannedEmployee.Badge;

                        ViewBag.scanEmpName = scannedEmployee.EmployeeFirstName + " " + scannedEmployee.EmployeeLastName;
                        ViewBag.time = DateTime.Now;
                        var empTime = ViewBag.time;


                        string[] values = null;
                        try
                        {
                            using (var reader = new StreamReader(fullPath))
                            {
                                var linee = reader.ReadLine();
                                values = linee.Split(',');
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                        }

                        //var name = values.ToArray();
                        var scannedEmployeeIndex = Array.IndexOf(values, scannedEmpName);
                        scannedEmployeeInd = scannedEmployeeIndex;

                        var count = 0;
                        var empCount = employeeList.Count();
                        // Creating Employee Clockin CSV file
                        while (count < empCount)
                        {
                            // Adding column names with employee names
                            badges.Add(employeeList.Select(c => c.EmployeeFirstName).ToList().ElementAt(count) + " " + employeeList.Select(c => c.EmployeeLastName).ToList().ElementAt(count));

                            count++;
                        }
                        //cw.NextRecord();
                        count = 0;
                        while (count < empCount)
                        {
                            // Adding column names with employee ID's
                            empName.Add(employeeList.Select(c => c.Badge).ToList().ElementAt(count));
                            count++;
                        }

                        if (lastClock[scannedEmployeeIndex] == "")
                        {
                            lastClock[scannedEmployeeIndex] = empTime.ToString("HH:mm:ss");
                        }
                        else
                        {
                            var currTime = DateTime.Now;
                            currTime = currTime.AddMinutes(-5);

                            var parsedDate = DateTime.Parse(lastClock[scannedEmployeeIndex]);

                            var currentTime = currTime.ToString("HH:mm:ss");
                            var g = DateTime.Compare(parsedDate, currTime);
                            System.Diagnostics.Debug.WriteLine(currTime);
                            if (g > 0)
                            {
                                wait = true;
                                ViewBag.time = "PLEASE WAIT 5 MINS. You are \nClocked " + inOutStatus[scannedEmployeeIndex];
                            }
                            else
                            {
                                wait = false;
                                lastClock[scannedEmployeeIndex] = empTime.ToString("HH:mm:ss");
                            }
                        }

                        //reading in from clock in file to add a clock in 
                        StreamReader sr = new StreamReader(fullPath);
                        var lines = new List<string[]>();
                        int Row = 0;
                        int colCount = 0;
                        while (!sr.EndOfStream)
                        {
                            string[] Line = sr.ReadLine().Split(',');
                            lines.Add(Line);
                            colCount = Line.Length;
                            Row++;
                        }
                        sr.Close();

                        var row = 0;
                        bool added = false;
                        while (row < lines.Count && wait == false)
                        {
                            if (lines[row][scannedEmployeeIndex].Equals(""))
                            {
                                DateTime now = empTime;
                                string asString = now.ToString("HH:mm:ss");
                                if (row % 2 == 0)
                                {
                                    inOut = "In";
                                }
                                else
                                {
                                    inOut = "Out";
                                }
                                //asString = String.Concat(inOut, asString);
                                asString = inOut + " " + asString;
                                ViewBag.time = ViewBag.time + " \nClocked " + inOut;
                                lines[row][scannedEmployeeIndex] = asString;
                                row = lines.Count;
                                added = true;
                            }
                            row++;

                            // if checked all row and data has not been added
                            if (row == lines.Count && added == false)
                            {
                                //System.Diagnostics.Debug.WriteLine("not added");
                                string[] Line = new string[colCount];
                                lines.Add(Line);

                                DateTime now = empTime;
                                string asString = now.ToString("HH:mm:ss");
                                if (row % 2 == 0)
                                {
                                    inOut = "In";
                                }
                                else
                                {
                                    inOut = "Out";
                                }
                                asString = inOut + " " + asString;
                                ViewBag.time = ViewBag.time + " \nClocked " + inOut;
                                lines[row][scannedEmployeeIndex] = asString;
                                row++;
                            }
                        }

                        //Writing out to file with updated clock ins
                        using (StreamWriter sw = new StreamWriter(fullPath, false, new UTF8Encoding(true)))
                        using (CsvWriter cw = new CsvWriter(sw, CultureInfo.InvariantCulture))
                        {
                            //cw.NextRecord();
                            int i = 0;
                            int j = 0;
                            int rowCount = lines.Count();

                            // Creating Employee Clockin CSV file
                            while (i < rowCount)
                            {
                                j = 0;
                                while (j < colCount)
                                {
                                    // Adding column names with employee ID's
                                    cw.WriteField(lines[i][j]);
                                    j++;
                                }
                                // Adding column names with employee names
                                cw.NextRecord();

                                i++;
                            }
                        }
                    }

                }
                else if (empBadge != null)
                {
                    ViewBag.time = "This user does not exist yet";
                    StartCounter();

                }
                if (wait == false)
                {
                    using (StreamWriter sw = new StreamWriter(badgePath, false, new UTF8Encoding(true)))
                    using (CsvWriter cw = new CsvWriter(sw, CultureInfo.InvariantCulture))
                    {
                        //cw.WriteField(DateTime.Now.DayOfWeek.ToString() + " " + date);
                        //cw.NextRecord();
                        int i = 0;
                        int empCount = employeeList.Count();

                        // Creating Employee Clockin CSV file
                        while (i < empCount)
                        {
                            // Adding column names with employee names
                            cw.WriteField(employeeList.Select(c => c.EmployeeFirstName).ToList().ElementAt(i) + " " + employeeList.Select(c => c.EmployeeLastName).ToList().ElementAt(i));

                            i++;
                        }
                        cw.NextRecord();
                        i = 0;
                        while (i < empCount)
                        {
                            // Adding column names with employee ID's
                            cw.WriteField(employeeList.Select(c => c.Badge).ToList().ElementAt(i));
                            i++;
                        }
                        cw.NextRecord();
                        i = 0;
                        while (i < empCount)
                        {
                            // Adding column names with employee ID's
                            cw.WriteField(lastClock[i]);
                            i++;
                        }
                        cw.NextRecord();
                        i = 0;
                        while (i < empCount)
                        {
                            // Adding column names with employee ID's
                            if (scannedEmployeeInd == i)
                            {
                                cw.WriteField(inOut);
                            }
                            else
                            {
                                cw.WriteField(inOutStatus[i]);
                            }
                            i++;
                        }

                    }
                }
                if (clear == true)
                {
                    ViewBag.time = "";
                    clear = false;
                }
                ModelState.Clear();
                return View();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                ViewBag.time = "";
                ViewBag.error = "Clock in failed. Please try again";
                ModelState.Clear();
                return View();
            }
        }

        public void StartCounter()
        {
            Timer aTimer;
            aTimer = new System.Timers.Timer(500);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = 500;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //ViewBag.time = "";
            //ViewBag.scanEmpName = "";
            System.Diagnostics.Debug.WriteLine("badge path.................... ");
            System.Diagnostics.Debug.WriteLine(e);
            clear = true;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
