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
            var errorRecordCount = 1;
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

                        while (csvReader.Read())
                        {
                            try
                            {
                                var record = csvReader.GetRecord<Customer>();
                                customerHashSet.Add(record); //// Only add valid records
                            }
                            catch (CsvHelper.FieldValidationException ex)
                            {
                                //// Skip this record and continue with the next one
                                Log.Logger.Error($"{errorRecordCount++}.....Skipping invalid record: {ex.Message}");
                            }
                        }

                        Log.Logger.Information($"Getting records from Csv file has finished....");

                        customerList = new List<Customer>(customerHashSet.ToList());
                        customerList.ForEach(customer => customer.PrepareDataBeforeInsertAndUpdate());

                    }
                }
            }
            catch(Exception ex)
            {
                Log.Logger.Error(ex, $"Error importing CSV file: {filePath}");
                throw;
            }

            if (customerList.Any())
            {
                new CustomerRepository().BulkInsertOrUpdateCustomersAsync(customerList);
            }
            else
            {
                Log.Logger.Warning($"There is not valid record to import....");
            }
        }
    }
}