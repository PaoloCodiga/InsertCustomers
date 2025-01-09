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
            //// Skip the record if the value is null or length is greater than 30
            Map(m => m.Code)
                .Validate(field => !string.IsNullOrEmpty(field.ToString()) && !string.IsNullOrWhiteSpace(field.ToString()) && field.ToString().Length <= 30)
                .Name("ID");

            //// Skip the record if the value is null or length is greater than 32
            Map(m => m.CardNumber)
                .Validate(field => !string.IsNullOrEmpty(field.ToString()) && !string.IsNullOrWhiteSpace(field.ToString()) && field.ToString().Length <= 32)
                .Name("ID");

            //// Skip the record if the value is null or length is greater than 40
            Map(m => m.Description)
                .Validate(field => !string.IsNullOrEmpty(field.ToString()) && field.ToString().Length <= 40)
                .Name("Customer");

            //// Skip the record if the length is greater than 40
            Map(m => m.Notes1)
                .Validate(field => field.ToString().Length <= 40)
                .Name("Comment");

            //// Skip the record if the value is null or value is not 1 and 2
            Map(m => m.CardType)
                .Validate(field => !string.IsNullOrEmpty(field.ToString()) && (Convert.ToInt16(field) == 1 || Convert.ToInt16(field) == 2))
                .Name("TypeOfCard");

            //// Skip the record if the value is not between 0 and 999999999999999.999
            Map(m => m.PrepayBalanceCash)
                .Validate(field => Convert.ToDecimal(field) >= 0 && Convert.ToDouble(field) <= 999999999999999.999)
                .Name("Balance")
                .Default(0);

            //// Skip the record if the value is not between 0 and 999999999999999.999
            Map(m => m.CreditBalance)
                .Validate(field => Convert.ToDecimal(field) >= 0 && Convert.ToDouble(field) <= 999999999999999.999)
                .Name("Balance")
                .Default(0);

            //// Skip the record if the value is not between 0 and 999999999999999.999
            Map(m => m.CreditLimit)
                .Validate(field => Convert.ToDecimal(field) >= 0 && Convert.ToDouble(field) <= 999999999999999.999)
                .Name("CreditLimit")
                .Default((decimal?)null);

            //// Skip the record if the length is greater than 20
            Map(m => m.FiscalCode)
                .Validate(field => field.ToString().Length <= 20)
                .Name("SalaryID");

            //// Skip the record if the length is greater than 100
            Map(m => m.Email)
                .Validate(field => field.ToString().Length <= 100)
                .Name("Email");
        }
    }
}
