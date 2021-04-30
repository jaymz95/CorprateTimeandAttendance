using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeissnerClockIn.Services
{
    public class TotalService
    {
        public void Total()
        {
            try
            {
                DateTime today = DateTime.Today.AddDays(-1);
                var date = today.ToString(@"ddMMMMyyyy");
                string file = date + "ClockIn.csv";
                string totalFile = date + "ClockInTotal.csv";
                string fullPath = Path.Combine(Environment.CurrentDirectory, @"TimeSheets", file);
                string totalPath = Path.Combine(Environment.CurrentDirectory, @"TimeSheets", totalFile);
                string[] empName, row0, row1;
                List<string> empId = new List<string>();
                var rows = 0;
                var i = 2;
                var j = 0;
                var col = 0;
                TimeSpan[] totals;

                if (System.IO.File.Exists(fullPath))
                {
                    rows = System.IO.File.ReadAllLines(fullPath).Length;
                    System.Diagnostics.Debug.WriteLine("\nYESTERDAY##################################### " + rows);
                    using (var reader = new StreamReader(fullPath, Encoding.Default))
                    {
                        empName = reader.ReadLine().Split(",");
                        empId = reader.ReadLine().Split(",").ToList();

                        col = empName.Length;
                        totals = new TimeSpan[col];

                        // read first 2 records
                        // loop through rows in each column
                        while (i <= rows - 2)
                        {
                            row0 = reader.ReadLine().Split(",");
                            row1 = reader.ReadLine().Split(",");
                            // loop through each employee column
                            while (j < col)
                            {
                                if (row0[j].Length > 8 && row1[j].Length > 8)
                                {
                                    System.Diagnostics.Debug.WriteLine("\n" + row0[j] + "is this thing on? " + row1[j] + "\t\t" + j + "\t\t" + rows);
                                    totals[j] += DateTime.Parse(row1[j].Substring(4)) - DateTime.Parse(row0[j].Substring(3));
                                    System.Diagnostics.Debug.WriteLine("\nTimeSpan " + totals[j]);
                                }

                                j++;
                            }
                            i = i + 2;
                            j = 0;
                        }

                    }
                    //Writing out to file with updated clock ins
                    using (StreamWriter sw = new StreamWriter(fullPath, true, new UTF8Encoding(true)))
                    using (CsvWriter cw = new CsvWriter(sw, CultureInfo.InvariantCulture))
                    {
                        int x = 0;

                        // Creating Employee Clockin CSV file
                        if (totals != null)
                        {
                            cw.WriteField("Totals:");
                            cw.NextRecord();
                            while (x < col)
                            {
                                cw.WriteField(totals[x]);
                                x++;
                            }
                        }
                    }
                    var result = File.ReadAllLines(fullPath).Select(x => x.Split(',')).ToArray();
                    System.IO.File.Move(fullPath, totalPath);

                }
            }catch(Exception e)
            {

            }
        }
    }
}
