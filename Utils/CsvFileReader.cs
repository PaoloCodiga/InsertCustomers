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
            Log.Logger.Information($"Reading Csv File....");

            //// using HashSet to avoid duplicate data in records
            var customerHashSet = new HashSet<Customer>();
            var customerList = new List<Customer>();
            var errorRecordCount = 1;
            var customerRepository = new CustomerRepository();

            try
            {
                var distinctCardNumberList = customerRepository.GetAllDistinctCardNumbers();
                var nextId = customerRepository.GetNextId();

                using (var streamReader = new StreamReader(filePath))
                {
                    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        //// Register the mapping class
                        csvReader.Context.RegisterClassMap<CustomerMap>();

                        Log.Logger.Information($"CsvReader Registering ClassMap has finished....");
                        Log.Logger.Information($"Getting records from Csv file....");

                        while (csvReader.Read())
                        {
                            try
                            {
                                var record = csvReader.GetRecord<Customer>();
                                record.PrepareDataBeforeInsertAndUpdate(!distinctCardNumberList.Contains(record.CardNumber), ref nextId);

                                //// Only add valid records
                                customerHashSet.Add(record);
                            }
                            catch (CsvHelper.FieldValidationException ex)
                            {
                                //// Skip this record and continue with the next one
                                Log.Logger.Error($"{errorRecordCount++}.....Skipping invalid record: {ex.Message}");
                            }
                        }

                        Log.Logger.Information($"Getting records from Csv file completes....");

                        customerList = new List<Customer>(customerHashSet.ToList());

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
                customerRepository.BulkInsertOrUpdateCustomers(customerList);
            }
            else
            {
                Log.Logger.Warning($"There is not valid record to import....");
            }
        }
    }
}