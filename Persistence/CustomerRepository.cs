using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TCPOS.InsertCustomers.Domain;

namespace TCPOS.InsertCustomers.Persistence
{
    public class CustomerRepository
    {
        /// <summary>
        /// SqlBulkCopy minimizes round trips to the database and can handle a large volume of data efficiently.
        /// Using transactions ensures that either all records are updated or none are, preserving consistency.
        /// </summary>
        /// <param name="customerList"></param>
        /// <returns></returns>
        public static async Task BulkInsertOrUpdateCustomersAsync(IList<Customer> customerList)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;
            var dataTable = CreateDataTable(customerList);

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                using (var sqlTransaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        //// Create temporary table
                        var createTempTableCommand = @"
                            CREATE TABLE #TempCustomers (
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
                            await sqlCommand.ExecuteNonQueryAsync();
                        }

                        //// Bulk copy data to temporary table
                        using (var bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, sqlTransaction))
                        {
                            bulkCopy.DestinationTableName = "#TempCustomers";
                            await bulkCopy.WriteToServerAsync(dataTable);
                        }

                        //// Merge temporary table with target (customers) table
                        //// The MERGE statement compares the records in the main table (customers) with the temporary table (#TempCustomers).
                        //// If a match is found(WHEN MATCHED), it updates the existing records.
                        //// If no match is found(WHEN NOT MATCHED BY TARGET), it inserts new records.
                        var mergeCommandText = @"
                            MERGE INTO Customers AS target
                            USING #TempCustomers AS source
                            ON target.card_num = source.card_num
                            WHEN MATCHED THEN
                                UPDATE SET 
                                    target.code = source.code,
                                    target.description = source.description
                                    target.notes1 = source.notes1
                                    target.card_type = source.card_type
                                    target.prepay_balance_cash = source.prepay_balance_cash
                                    target.credit_balance = source.credit_balance
                                    target.credit_limit = source.credit_limit
                                    target.fiscal_code = source.fiscal_code
                                    target.email = source.email
                            WHEN NOT MATCHED BY TARGET THEN
                                INSERT (
                                    code,
                                    card_num,
                                    description,
                                    notes1,
                                    card_type,
                                    prepay_balance_cash,
                                    credit_balance,
                                    credit_limit,
                                    fiscal_code,
                                    email )
                                VALUES (
                                    source.code,
                                    source.card_num,
                                    source.description,
                                    source.notes1,
                                    source.card_type,
                                    source.prepay_balance_cash,
                                    source.credit_balance,
                                    source.credit_limit,
                                    source.fiscal_code,
                                    source.email );";

                        using (var mergeCommand = new SqlCommand(mergeCommandText, sqlConnection, sqlTransaction))
                        {
                            await mergeCommand.ExecuteNonQueryAsync();
                        }

                        //// Commit transaction
                        //// Bulk insert and update completed successfully.
                        sqlTransaction.Commit();
                    }
                    catch
                    {
                        //// Rollback transaction when error encountered
                        sqlTransaction.Rollback();
                    }
                }
            }
        }

        private static DataTable CreateDataTable(IList<Customer> customerList)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("code", typeof(string));
            dataTable.Columns.Add("card_num", typeof(string));
            dataTable.Columns.Add("description", typeof(string));
            dataTable.Columns.Add("notes1", typeof(string));
            dataTable.Columns.Add("card_type", typeof(int));
            dataTable.Columns.Add("prepay_balance_cash", typeof(decimal));
            dataTable.Columns.Add("credit_balance", typeof(decimal));
            dataTable.Columns.Add("credit_limit", typeof(decimal?));
            dataTable.Columns.Add("fiscal_code", typeof(string));
            dataTable.Columns.Add("email", typeof(string));

            //// Map each customer to DataRow
            foreach (var customer in customerList)
            {
                dataTable.Rows.Add(MapToRow(customer));
            }

            return dataTable;
        }

        private static DataRow MapToRow(Customer customer)
        {
            var row = new DataTable().NewRow();
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
