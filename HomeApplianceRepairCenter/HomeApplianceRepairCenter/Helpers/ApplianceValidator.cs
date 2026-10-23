using System;
using System.Text.RegularExpressions;

namespace HomeApplianceRepairCenter.Helpers
{
    /// <summary>
    /// Static class with input-validation helpers (no instance state needed).
    /// </summary>
    public static class ApplianceValidator
    {
        // Serial format: 2-3 capital letters, a dash, then 6 digits (e.g. RF-100234)
        private static readonly Regex SerialPattern =
            new Regex(@"^[A-Z]{2,3}-\d{6}$", RegexOptions.Compiled);

        public static bool IsValidSerialNumber(string serialNumber)
        {
            return !string.IsNullOrWhiteSpace(serialNumber) && SerialPattern.IsMatch(serialNumber);
        }

        public static bool IsValidName(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Trim().Length >= 2;
        }

        public static bool IsValidCost(decimal amount)
        {
            return amount >= 0;
        }

        /// <summary>The device cannot be received in the future or before the year 2000.</summary>
        public static bool IsValidReceivedDate(DateTime receivedDate)
        {
            return receivedDate.Year >= 2000 && receivedDate.Date <= DateTime.Today;
        }

        /// <summary>The promised delivery date cannot be before the reception date.</summary>
        public static bool IsValidDeliveryDate(DateTime receivedDate, DateTime expectedDeliveryDate)
        {
            return expectedDeliveryDate.Date >= receivedDate.Date;
        }
    }
}
