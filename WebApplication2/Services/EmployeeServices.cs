using CsvHelper;
using MeissnerClockIn.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeissnerClockIn.Services
{
    public class EmployeeService
    {
        public List<Employee> ReadCSVFile(string location)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(location);
                using (var reader = new StreamReader(location, Encoding.Default))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<EmployeeMap>();
                    var records = csv.GetRecords<Employee>().ToList();
                    return records;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void WriteCSVFile(string path, List<Employee> employee)
        {
            using (StreamWriter sw = new StreamWriter(path, false, new UTF8Encoding(true)))
            using (CsvWriter cw = new CsvWriter(sw, CultureInfo.InvariantCulture))
            {
                cw.WriteHeader<Employee>();
                cw.NextRecord();
                foreach (Employee emp in employee)
                {
                    cw.WriteRecord<Employee>(emp);
                    cw.NextRecord();
                }
            }
        }
    }
}
