using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace InsertCustomers
{
    public class CsvFileReader
    {
        public static void ReadCsvFile(string filePath)
        {
            var records = new List<Customer>();
            using (var streamReader = new StreamReader(filePath))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    records = csvReader.GetRecords<Customer>().ToList();
                }
            }

            //// Validate and repair each property of objects from the records
            foreach (var record in records)
            {

            }
        }
    }
}