
namespace ZimraEGS.ApiClient.Helpers
{
    public static class ValidationText
    {
        public static Dictionary<string, string> ErrorString = new Dictionary<string, string>
        {
            { "RCPT010", "Wrong currency code is used" },
            { "RCPT011", "Receipt counter is not sequential." },
            { "RCPT012", "Receipt global number is not sequential." },
            { "RCPT013", "Invoice number is not unique" },
            { "RCPT014", "Receipt date is earlier than fiscal day opening date" },
            { "RCPT015", "Credited/debited invoice data is not provided" },
            { "RCPT016", "No receipt lines provided" },
            { "RCPT017", "Taxes information is not provided" },
            { "RCPT018", "Payment information is not provided" },
            { "RCPT019", "Invoice total amount is not equal to sum of all invoice lines" },
            { "RCPT020", "Invoice signature is not valid" },
            { "RCPT021", "VAT tax is used in invoice while taxpayer is not VAT taxpayer" },
            { "RCPT022", "Invoice sales line price must be greater than 0 (less than 0 for Credit note), discount line price must be less than 0 (greater than 0 for Credit note)" },
            { "RCPT023", "Invoice line quantity, must be positive" },
            { "RCPT024", "Invoice line total is not equal to unit price * quantity" },
            { "RCPT025", "Invalid tax is used" },
            { "RCPT026", "Incorrectly calculated tax amount" },
            { "RCPT027", "Incorrectly calculated total sales amount (including tax)" },
            { "RCPT028", "Payment amount must be greater than or equal 0 (less than or equal to 0 for Credit note)" },
            { "RCPT029", "Credited/debited invoice information provided for regular invoice" },
            { "RCPT030", "Invoice date is earlier than previously submitted receipt date" },
            { "RCPT031", "Invoice is submitted with the future date" },
            { "RCPT032", "Credit / debit note refers to non-existing invoice" },
            { "RCPT033", "Credited/debited invoice is issued more than 12 months ago" },
            { "RCPT034", "Note for credit/debit note is not provided" },
            { "RCPT035", "Total credit note amount exceeds original invoice amount" },
            { "RCPT036", "Credit/debit note uses other taxes than are used in the original invoice" },
            { "RCPT037", "Invoice total amount is not equal to sum of all invoice lines and taxes applied" },
            { "RCPT038", "Invoice total amount is not equal to sum of sales amount including tax in tax table" },
            { "RCPT039", "Invoice total amount is not equal to sum of all payment amounts" },
            { "RCPT040", "Invoice total amount must be greater than or equal to 0 (less than or equal to 0 for Credit note)" },
            { "RCPT041", "Invoice is issued after fiscal day end" },
            { "RCPT042", "Credit/debit note uses other currency than is used in the original invoice" }
        };

        public static string GetErrorString(string validationErrorCode)
        {
            // Check if the dictionary contains the key and return the corresponding value, else return a default message.
            if (ErrorString.TryGetValue(validationErrorCode, out string errorMessage))
            {
                return errorMessage;
            }
            return "Unknown error code";
        }

    }
}
