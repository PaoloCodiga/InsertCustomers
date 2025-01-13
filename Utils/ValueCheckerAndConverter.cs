using System;

namespace TCPOS.InsertCustomers.Utils
{
    public static class ValueCheckerAndConverter
    {
        /// <param name="value"></param>
        /// <returns>Return value with three decimal places if the value has more than three decimal places.
        /// Otherwise return the original value</returns>
        public static decimal? ConcactToThreeDecimalPlaces(string input) => string.IsNullOrEmpty(input)
            ? default(decimal?)
            : decimal.Parse(InnerConcactToThreeDecimalPlaces(input));

        /// <summary>
        /// Check whether the string is null or empty or white space
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrWhiteSpace(string input) =>
            string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input);

        /// <summary>
        /// Check whether the valueToCheck is withing the range (upperBound and lowerBound)
        /// </summary>
        /// <param name="valueToCheck"></param>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns>true if between the range</returns>
        public static bool IsBetweenRange(decimal valueToCheck, decimal lowerBound, decimal upperBound) =>
            valueToCheck.CompareTo(lowerBound) >= 0 && valueToCheck.CompareTo(upperBound) <= 0;

        private static string InnerConcactToThreeDecimalPlaces(string input)
        {
            //// Find the index of the decimal point
            var decimalIndex = input.IndexOf('.');

            //// If no decimal point found, return the original string
            if (decimalIndex == -1)
            {
                return input;
            }

            //// Calculate the end index for concatenation (3 places after the decimal)
            var endIndex = Math.Min(decimalIndex + 4, input.Length);

            //// Concatenate the string from the beginning to the calculated end index
            return input.Substring(0, endIndex);
        }
    }
}
