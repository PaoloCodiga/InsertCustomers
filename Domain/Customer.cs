﻿using TCPOS.InsertCustomers.Utils;

namespace TCPOS.InsertCustomers.Domain
{
    public class Customer
    {
        public int Id { get; set; } = -1;

        public string Code { get; set; }

        public string CardNumber { get; set; }

        public string Description { get; set; }

        public string Notes1 { get; set; }

        public int CardType { get; set; }

        public decimal PrepayBalanceCash { get; set; }

        public decimal CreditBalance { get; set; }

        public decimal CreditLimit { get; set; }

        public string FiscalCode { get; set; }

        public string Email { get; set; }

        public void PrepareDataBeforeInsertAndUpdate(bool isInsertData, ref int nextId)
        {
            if (isInsertData)
            {
                this.Id = nextId++;
            }

            //// Preperation of Balance according to CardType
            if (this.CardType == 1)
            {
                this.PrepayBalanceCash = 0;
                this.CreditBalance = ValueCheckerAndConverter.ConcactToThreeDecimalPlaces(this.CreditBalance.ToString());
            }
            else
            {
                this.CreditBalance = 0;
                this.PrepayBalanceCash = ValueCheckerAndConverter.ConcactToThreeDecimalPlaces(this.PrepayBalanceCash.ToString());
            }

            this.CreditLimit = ValueCheckerAndConverter.ConcactToThreeDecimalPlaces(this.CreditLimit.ToString());
        }

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
