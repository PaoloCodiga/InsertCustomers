using CsvHelper.Configuration;
using System;

namespace TCPOS.InsertCustomers.Domain
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            //// Skip the record if the value is null or length is greater than 30
            Map(m => m.Code)
                .Validate(field => !string.IsNullOrEmpty(field.Field)
                    && !string.IsNullOrWhiteSpace(field.Field)
                    && field.Field.Length <= 30)
                .Name("ID");

            //// Skip the record if the value is null or length is greater than 32
            Map(m => m.CardNumber)
                .Validate(field => !string.IsNullOrEmpty(field.Field) 
                    && !string.IsNullOrWhiteSpace(field.Field) 
                    && field.Field.Length <= 32)
                .Name("ID");

            //// Skip the record if the value is null or length is greater than 40
            Map(m => m.Description)
                .Validate(field => !string.IsNullOrEmpty(field.Field) 
                    && field.Field.Length <= 40)
                .Name("Customer");

            //// Skip the record if the length is greater than 40
            Map(m => m.Notes1)
                .Validate(field => field.Field.Length <= 40)
                .Name("Comment");

            // Skip the record if the value is null or value is not 1 and 2
            Map(m => m.CardType)
                .Validate(field => !string.IsNullOrEmpty(field.Field) 
                    && (Convert.ToInt32(field.Field) == 1 || Convert.ToInt32(field.Field) == 2))
                .Name("TypeOfCard");

            //// Skip the record if the value is not between 0 and 999999999999999.999
            Map(m => m.PrepayBalanceCash)
                .Validate(field => Convert.ToDecimal(this.Get0IfStringIsNullOrEmpty(field.Field)) >= 0 
                    && Convert.ToDouble(this.Get0IfStringIsNullOrEmpty(field.Field)) <= 999999999999999.999)
                .Name("Balance")
                .Default(0);

            //// Skip the record if the value is not between 0 and 999999999999999.999
            Map(m => m.CreditBalance)
                .Validate(field => Convert.ToDecimal(this.Get0IfStringIsNullOrEmpty(field.Field)) >= 0 
                    && Convert.ToDouble(this.Get0IfStringIsNullOrEmpty(field.Field)) <= 999999999999999.999)
                .Name("Balance")
                .Default(0);

            //// Skip the record if the value is not between 0 and 999999999999999.999
            Map(m => m.CreditLimit)
                .Validate(field => Convert.ToDecimal(this.Get0IfStringIsNullOrEmpty(field.Field)) >= 0 
                    && Convert.ToDouble(this.Get0IfStringIsNullOrEmpty(field.Field)) <= 999999999999999.999)
                .Name("CreditLimit")
                .Default((decimal?)null);

            //// Skip the record if the length is greater than 20
            Map(m => m.FiscalCode)
                .Validate(field => field.Field.Length <= 20)
                .Name("SalaryID");

            //// Skip the record if the length is greater than 100
            Map(m => m.Email)
                .Validate(field => field.Field.Length <= 100)
                .Name("Email");
        }

        private string Get0IfStringIsNullOrEmpty(string input) => string.IsNullOrEmpty(input) ? "0" : input; 
    }
}