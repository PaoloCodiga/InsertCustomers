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
        private ILogger logger;

        public CsvFileReader(ILogger logger)
        {
            this.logger = logger;
        }

        public void ReadCsvFile(string filePath)
        {
            //// using HashSet to avoid duplicate data in records
            var customerHashSet = new HashSet<Customer>();
            try
            {
                var a = Convert.ToInt16(customerHashSet.First().Email);
                using (var streamReader = new StreamReader(filePath))
                {
                    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        csvReader.Context.RegisterClassMap<CustomerMap>(); //// Register the mapping class

                        customerHashSet = new HashSet<Customer>(csvReader.GetRecords<Customer>().ToList());
                        var customerList = new List<Customer>(customerHashSet.ToList());
                        
                        this.PrepareDataBeforeInsertAndUpdate(customerList);
                        CustomerRepository.BulkInsertOrUpdateCustomersAsync(customerList).Wait();
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Error importing CSV file: {filePath}");
                throw;
            }
        }

        private void PrepareDataBeforeInsertAndUpdate(IList<Customer> customerList)
        {
            //// Validate and repair each property of objects from the records
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