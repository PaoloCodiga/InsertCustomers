using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsertCustomers
{
    public class Customer
    {
        public string Code { get; set; }

        public string CardNumber { get; set; }

        public string Description { get; set; }

        public string Notes1 { get; set; }

        public int CardType { get; set; }

        public decimal PrepayBalanceCash { get; set; }

        public decimal CreditBalance { get; set; }

        public decimal? CreditLimit { get; set; }

        public string FiscalCode { get; set; }

        public string Email { get; private set; }

        public RecordStatusEnum RecordStatus { get; set; } = RecordStatusEnum.Insert;

    }
}
