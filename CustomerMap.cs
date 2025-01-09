using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Drawing.Text;
using System.Linq;

namespace InsertCustomers
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Map(m => m.Code)
                .Validate(field => string.IsNullOrEmpty(field.ToString()) && string.IsNullOrEmpty(field.ToString()) && field.ToString().Length <= 30) // Max length 30 and not null data
                .Name("ID");

            Map(m => m.CardNumber)
                .Validate(field => string.IsNullOrEmpty(field.ToString()) && string.IsNullOrEmpty(field.ToString()) && field.ToString().Length <= 30) // Max length 30 and not null data
                .Name("ID");

            Map(m => m.Description)
                .Validate(field => string.IsNullOrEmpty(field.ToString()) && field.ToString().Length <= 40) // Max length 40 and not null data
                .Name("Customer");

            Map(m => m.Notes1)
                .Validate(field => field.ToString().Length <= 40) // Max length 40
                .Name("Comment");

            Map(m => m.CardType)
                .Validate(field => Convert.ToInt16(field) == 1 || Convert.ToInt16(field) == 2) // Allowed Card Type is 1 or 2
                .Name("TypeOfCard");

            Map(m => m.CardType)
                .Validate(field => new int[] { 1, 2 }.Contains(field)) // Allowed Card Type is 1 or 2
                .Name("TypeOfCard");

            Map(m => m.PrepayBalanceCash)
                .Validate(field => Convert.ToDecimal(field) >= 0 && Convert.ToDouble(field) <= 999999999999999.999) // value range should be between 0 and 999999999999999.999
                .Name("Balance")
                .Default(0);

            Map(m => m.CreditBalance)
                .Validate(field => Convert.ToDecimal(field) >= 0 && Convert.ToDouble(field) <= 999999999999999.999) // value range should be between 0 and 999999999999999.999
                .Name("Balance")
                .Default(0);

            Map(m => m.CreditLimit)
                .Validate(field => Convert.ToDecimal(field) >= 0 && Convert.ToDouble(field) <= 999999999999999.999) // value range should be between 0 and 999999999999999.999
                .Name("CreditLimit")
                .Default((decimal?)null);

            [Name("SalaryID")]
            public string FiscalCode { get; set; }

            [Name("Email")]
            public string Email { get; private set; }
        }
    }
}
