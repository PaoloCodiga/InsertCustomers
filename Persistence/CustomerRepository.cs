using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TCPOS.InsertCustomers.Domain;

namespace TCPOS.InsertCustomers.Persistence
{
    public class CustomerRepository
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
        readonly string uniqueKeyStoredProcedureName = "make_unique_key";

        /// <summary>
        /// SqlBulkCopy minimizes round trips to the database and can handle a large volume of data efficiently.
        /// Using transactions ensures that either all records are updated or none are, preserving consistency.
        /// </summary>
        /// <param name="customerList"></param>
        /// <returns></returns>
        public void BulkInsertOrUpdateCustomers(IList<Customer> customerList)
        {
            Log.Logger.Information($"Trying to insert/update data into customers database....");
            var dataTable = this.CreateDataTable(customerList);

            using (var sqlConnection = new SqlConnection(this.connectionString))
            {
                sqlConnection.Open();

                Log.Logger.Information($"Open Sql Connection ....");
                using (var sqlTransaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        //// Create temporary table
                        var createTempTableCommand = @"
                            CREATE TABLE #TempCustomers (
                                id int,
                                code varchar(30) NULL,
                                card_num varchar(32) NULL,
                                description varchar(40) NOT NULL,
                                notes1 varchar(40) NULL,
                                card_type numeric(1, 0) NOT NULL,
                                prepay_balance_cash numeric(15, 3) NOT NULL,
                                credit_balance numeric(15, 3) NOT NULL,
                                credit_limit numeric(15, 3) NULL,
                                fiscal_code varchar(20) NULL,
                                email varchar(100) NULL)";

                        using (var sqlCommand = new SqlCommand(createTempTableCommand, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.ExecuteNonQuery();
                        }

                        Log.Logger.Information($"Creating TempTable Completes....");

                        //// Bulk copy data to temporary table
                        using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
                        {
                            bulkCopy.DestinationTableName = "#TempCustomers";

                            Log.Logger.Information($"Copying data to TempTable....");

                            bulkCopy.WriteToServer(dataTable);
                        }

                        Log.Logger.Information($"Copying data to TempTable Completes");

                        //// Merge temporary table with target (customers) table
                        //// The MERGE statement compares the records in the main table (customers) with the temporary table (#TempCustomers).
                        //// If a match is found(WHEN MATCHED), it updates the existing records.
                        //// If no match is found(WHEN NOT MATCHED BY TARGET), it inserts new records.
                        //// Update currval of ad_sequences
                        var mergeCommandText = @"
                            MERGE INTO customers AS target
                            USING #TempCustomers AS source
                            ON target.card_num = source.card_num COLLATE Latin1_General_CI_AS
                            WHEN MATCHED THEN
                                UPDATE SET 
                                    target.code = source.code,
                                    target.description = source.description,
                                    target.notes1 = source.notes1,
                                    target.card_type = source.card_type,
                                    target.prepay_balance_cash = source.prepay_balance_cash,
                                    target.credit_balance = source.credit_balance,
                                    target.credit_limit = source.credit_limit,
                                    target.fiscal_code = source.fiscal_code,
                                    target.email = source.email
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (
                                    id,
                                    code,
                                    card_num,
                                    description,
                                    notes1,
                                    card_type,
                                    prepay_balance_cash,
                                    credit_balance,
                                    credit_limit,
                                    fiscal_code,
                                    email,
                                    visibility_criteria_id,
                                    is_valid,
                                    prepay_payment_id,
                                    prepay_balance_voucher,
                                    prepay_balance_bonus,
                                    credit_payment_id,
                                    no_manual_input,
                                    balance_on_card,
                                    language,
                                    passwd )
                                VALUES (
                                    source.id,
                                    source.code,
                                    source.card_num,
                                    source.description,
                                    source.notes1,
                                    source.card_type,
                                    source.prepay_balance_cash,
                                    source.credit_balance,
                                    source.credit_limit,
                                    source.fiscal_code,
                                    source.email,
                                    1,
                                    1,
                                    4,
                                    0,
                                    0,
                                    4,
                                    1,
                                    0,
                                    0,
                                    '*' );

                                DROP TABLE #TempCustomers;

                        UPDATE ad_sequences 
                        SET currval = (SELECT MAX(id) FROM customers)
                        WHERE table_name = 'customers';";

                        using (var mergeCommand = new SqlCommand(mergeCommandText, sqlConnection, sqlTransaction))
                        {
                            Log.Logger.Information($"Merging data to Customers table....");
                            mergeCommand.ExecuteNonQuery();
                            Log.Logger.Information($"Merging data to Customers table completes....");
                        }

                        //// Commit transaction
                        //// Bulk insert and update completed successfully.
                        sqlTransaction.Commit();
                        Log.Logger.Information($"Committed Sql Transaction....");
                    }
                    catch(Exception ex)
                    {
                        Log.Logger.Error(ex, $"Error occured....");

                        //// Rollback transaction when error encountered
                        sqlTransaction.Rollback();

                        Log.Logger.Information($"");
                        Log.Logger.Information($"Rollback Sql Transaction completed....");
                        throw;
                    }
                }
            }
        }

        public List<string> GetAllDistinctCardNumbers()
        {
            Log.Logger.Information($"Getting distinct CardNumbers....");

            var cardNumberList = new List<string>();
            using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
            {
                sqlConnection.Open();

                string query = $"SELECT DISTINCT card_num FROM customers";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        cardNumberList.Add(sqlDataReader.GetString(0));
                    }
                }
            }

            Log.Logger.Information($"Getting distinct CardNumbers completes....");
            return cardNumberList;
        }

        public int GetNextId()
        {
            Log.Logger.Information($"Getting next id....");
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(this.uniqueKeyStoredProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@table_name", "customers");
                    SqlParameter outputParameter = new SqlParameter("@nextval", SqlDbType.Int);
                    outputParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outputParameter);
                    command.ExecuteNonQuery();

                    Log.Logger.Information($"Getting next id completes....");
                    return (int)outputParameter.Value;
                }
            }
        }

        private DataTable CreateDataTable(IList<Customer> customerList)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("id", typeof(int));
            dataTable.Columns.Add("code", typeof(string));
            dataTable.Columns.Add("card_num", typeof(string));
            dataTable.Columns.Add("description", typeof(string));
            dataTable.Columns.Add("notes1", typeof(string));
            dataTable.Columns.Add("card_type", typeof(int));
            dataTable.Columns.Add("prepay_balance_cash", typeof(decimal));
            dataTable.Columns.Add("credit_balance", typeof(decimal));
            dataTable.Columns.Add("credit_limit", typeof(decimal));
            dataTable.Columns.Add("fiscal_code", typeof(string));
            dataTable.Columns.Add("email", typeof(string));

            //// Map each customer to DataRow
            foreach (var customer in customerList)
            {
                dataTable.Rows.Add(this.MapToRow(customer, dataTable));
            }

            return dataTable;
        }

        private DataRow MapToRow(Customer customer, DataTable dataTable)
        {
            var row = dataTable.NewRow();
            row["id"] = customer.Id;
            row["code"] = customer.Code;
            row["card_num"] = customer.CardNumber;
            row["description"] = customer.Description;
            row["notes1"] = customer.Notes1;
            row["card_type"] = customer.CardType;
            row["prepay_balance_cash"] = customer.PrepayBalanceCash;
            row["credit_balance"] = customer.CreditBalance;
            row["credit_limit"] = customer.CreditLimit;
            row["fiscal_code"] = customer.FiscalCode;
            row["email"] = customer.Email;
            return row;
        }
    }
}
