using CsvHelper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TCPOS.InsertCustomers.Persistence;

namespace TCPOS.InsertCustomers.Domain
{
    public class CsvFileReader
    {
        public void ReadCsvFile(string filePath)
        {
            Log.Logger.Information($"Reading Csv File starts....");

            //// using HashSet to avoid duplicate data in records
            var customerHashSet = new HashSet<Customer>();
            var customerList = new List<Customer>();

            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        //// Register the mapping class
                        csvReader.Context.RegisterClassMap<CustomerMap>();

                        Log.Logger.Information($"CsvReader Registering ClassMap has finished....");
                        Log.Logger.Information($"Getting records from Csv file starts....");

                        customerHashSet = new HashSet<Customer>(csvReader.GetRecords<Customer>().ToList());

                        Log.Logger.Information($"Getting records from Csv file has finished....");

                        customerList = new List<Customer>(customerHashSet.ToList());
                        this.PrepareDataBeforeInsertAndUpdate(customerList);
                        
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Logger.Error(ex, $"Error importing CSV file: {filePath}");
                throw;
            }

            new CustomerRepository().BulkInsertOrUpdateCustomersAsync(customerList);
        }

        private void PrepareDataBeforeInsertAndUpdate(IList<Customer> customerList)
        {
            //// Preperation of Balance according to CardType
            foreach (var customer in customerList)
            {
                if (customer.CardType == 1)
                {
                    customer.PrepayBalanceCash = 0;
                }
                else
                {
                    customer.CreditBalance = 0;
                }
            }
        }
    }
}