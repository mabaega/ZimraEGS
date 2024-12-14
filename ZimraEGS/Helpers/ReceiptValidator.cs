using ZimraEGS.ApiClient.Models;

namespace ZimraEGS.Helpers
{
    public class ReceiptValidator
    {
        public static string ValidateReceipt(Receipt receipt, string ApplicableTaxes)
        {
            List<string> errors = new List<string>();

            var acceptedCurrencyCodes = new List<string> { "ZWG", "USD" };

            if (!acceptedCurrencyCodes.Contains(receipt.ReceiptCurrency))
            {
                errors.Add("RCPT010: Wrong currency code is used");
            }

            if (receipt.ReceiptLines == null || !receipt.ReceiptLines.Any())
            {
                errors.Add("RCPT016: No receipt lines provided");
            }

            if (receipt.ReceiptTaxes == null || !receipt.ReceiptTaxes.Any())
            {
                errors.Add("RCPT017: Taxes information is not provided");
            }
            else
            {
                //string ApplicableTaxes = "1 - Exempt - #2 - Zero rated 0% - 0#3 - Standard rated 15% - 15#514 - Non-VAT Withholding Tax - 5";
                var validTaxIDs = ApplicableTaxes.Split('#')
                    .Select(part => part.Split(' ')[0])
                    .Where(id => int.TryParse(id, out _))
                    .Select(int.Parse)
                    .ToList();

                foreach (var tax in receipt.ReceiptTaxes)
                {
                    if (!validTaxIDs.Contains(tax.TaxID))
                    {
                        errors.Add("RCPT025: Invalid tax is used");
                    }
                }
            }

            if (receipt.ReceiptPayments == null || !receipt.ReceiptPayments.Any())
            {
                errors.Add("RCPT018: Payment information is not provided");
            }
            else if (receipt.ReceiptPayments.Sum(x => x.PaymentAmount) != receipt.ReceiptTotal)
            {
                errors.Add("RCPT039: Invoice total amount is not equal to sum of all payment amounts");
            }

            if (receipt.ReceiptType != ApiClient.Enums.ReceiptType.FiscalInvoice)
            {
                if (receipt.CreditDebitNote != null)
                {
                    // Check if the credited/debited invoice is issued more than 12 months ago
                    if (receipt.ReceiptDate - receipt.CreditDebitNote.ReceiptRefDate > TimeSpan.FromDays(365))
                    {
                        errors.Add("RCPT033: Credited/debited invoice is issued more than 12 months ago");
                    }

                    // Check if the credit/debit note refers to a non-existing invoice
                    if (receipt.CreditDebitNote.ReceiptID == 0)
                    {
                        errors.Add("RCPT032: Credit / debit note refers to non-existing invoice");
                    }
                }
                else
                {
                    // Handle case where the credit/debit note is not provided
                    errors.Add("RCPT034: Note for credit/debit note is not provided");
                }
            }

            if (errors.Count > 0)
            {
                return string.Join("\n", errors);
            }

            return string.Empty;
        }
    }
}