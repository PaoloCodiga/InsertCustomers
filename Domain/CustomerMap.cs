using CsvHelper.Configuration;
using TCPOS.InsertCustomers.Utils;

namespace TCPOS.InsertCustomers.Domain
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            //// Skip the record if the value is null or length is greater than 30
            Map(m => m.Code)
                .Validate(field => !ValueCheckerAndConverter.IsNullOrEmptyOrWhiteSpace(field.Field) && field.Field.Length <= 30)
                .Name("ID");

            //// Skip the record if the value is null or length is greater than 32
            Map(m => m.CardNumber)
                .Validate(field => !ValueCheckerAndConverter.IsNullOrEmptyOrWhiteSpace(field.Field) && field.Field.Length <= 32)
                .Name("ID");

            //// Skip the record if the value is null or length is greater than 40
            Map(m => m.Description)
                .Validate(field => !string.IsNullOrEmpty(field.Field) 
                    && field.Field.Length <= 40)
                .Name("Customer");

            //// Skip the record if the length is greater than 40
            Map(m => m.Notes1)
                .Validate(field => field.Field.Length <= 40)
                .Name("Comment")
                .Default((string)null);

            //// Skip the record if the value is null or empty or white space or not 1 and 2
            Map(m => m.CardType)
                .Validate(field => !ValueCheckerAndConverter.IsNullOrEmptyOrWhiteSpace(field.Field)
                    && int.TryParse(field.Field, out _)
                    && (int.Parse(field.Field) == 1 || int.Parse(field.Field) == 2))
                .Name("TypeOfCard");

            //// Skip the record if the value is null or empty or alphabet or value is not between 0 and 999999999999.999
            Map(m => m.PrepayBalanceCash)
                .Validate(field => string.IsNullOrEmpty(field.Field)
                    || (decimal.TryParse(field.Field, out _)
                       && ValueCheckerAndConverter.IsBetweenRange(decimal.Parse(field.Field), 0M, 999999999999.999M)))
                .Name("Balance")
                .Default(0);

            //// Skip the record if the value is null or empty or alphabet or value is not between 0 and 999999999999.999
            Map(m => m.CreditBalance)
                .Validate(field => string.IsNullOrEmpty(field.Field)
                    || (decimal.TryParse(field.Field, out _)
                        && ValueCheckerAndConverter.IsBetweenRange(decimal.Parse(field.Field), 0M, 999999999999.999M)))
                .Name("Balance")
                .Default(0);

            //// Skip the record if the value is white space or not between 0 and 999999999999.999
            Map(m => m.CreditLimit)
                .Validate(field => string.IsNullOrEmpty(field.Field)
                    || (decimal.TryParse(field.Field, out _)
                        && ValueCheckerAndConverter.IsBetweenRange(decimal.Parse(field.Field), 0M, 999999999999.999M)))
                .Name("CreditLimit")
                .Default(0M);

            //// Skip the record if the length is greater than 20
            Map(m => m.FiscalCode)
                .Validate(field => field.Field.Length <= 20)
                .Name("SalaryID")
                .Default((string)null);

            //// Skip the record if the length is greater than 100
            Map(m => m.Email)
                .Validate(field => field.Field.Length <= 100)
                .Name("Email")
                .Default((string)null);
        }
    }
}