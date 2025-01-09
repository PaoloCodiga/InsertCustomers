namespace TCPOS.InsertCustomers.Domain
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

        public override bool Equals(object obj)
        {
            if (obj is Customer other)
            {
                return CardNumber == other.CardNumber; //// Compare based on CardNumber
            }
            return false;
        }

        public override int GetHashCode()
        {
            return CardNumber.GetHashCode(); //// Use CardNumber for hash code generation
        }

    }
}
