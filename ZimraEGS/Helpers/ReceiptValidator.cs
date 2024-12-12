using ZimraEGS.ApiClient.Models;

public class ReceiptValidator
{
    public static List<string> ValidateReceipt(Receipt receipt, DateTime fiscalDayOpened)
    {
        List<string> errors = new List<string>();

        // RCPT010: Wrong currency code is used
        if (receipt.ReceiptCurrency != "ZWG")
        {
            errors.Add("RCPT010: Wrong currency code is used");
        }

        // RCPT014: Receipt date is earlier than fiscal day opening date
        if (receipt.ReceiptDate < fiscalDayOpened)
        {
            errors.Add("RCPT014: Receipt date is earlier than fiscal day opening date");
        }

        // RCPT015: Credited/debited invoice data is not provided
        // Specific to credit/debit notes, implementation depends on additional context

        // RCPT016: No receipt lines provided
        if (receipt.ReceiptLines == null || !receipt.ReceiptLines.Any())
        {
            errors.Add("RCPT016: No receipt lines provided");
        }

        // RCPT017: Taxes information is not provided
        if (receipt.ReceiptTaxes == null || !receipt.ReceiptTaxes.Any())
        {
            errors.Add("RCPT017: Taxes information is not provided");
        }

        // RCPT018: Payment information is not provided
        if (receipt.ReceiptPayments == null || !receipt.ReceiptPayments.Any())
        {
            errors.Add("RCPT018: Payment information is not provided");
        }

        // RCPT019: Invoice total amount is not equal to sum of all invoice lines
        double sumInvoiceLines = receipt.ReceiptLines.Sum(line => line.ReceiptLineTotal);
        if (Math.Abs(sumInvoiceLines - receipt.ReceiptTotal) > 0.01)
        {
            errors.Add("RCPT019: Invoice total amount is not equal to sum of all invoice lines");
        }

        if (string.IsNullOrEmpty(receipt.BuyerData.VatNumber))
        {
            errors.Add("RCPT021: VAT tax is used in invoice while taxpayer is not VAT taxpayer");
        }

        // RCPT022: Invoice sales line price must be greater than 0, etc.
        //foreach (var line in receipt.ReceiptLines)
        //{
        //    if (line.ReceiptLinePrice <= 0)
        //    {
        //        errors.Add("RCPT022: Invoice sales line price must be greater than 0");
        //    }
        //}

        //// RCPT023: Invoice line quantity must be positive
        //foreach (var line in receipt.ReceiptLines)
        //{
        //    if (line.ReceiptLineQuantity <= 0)
        //    {
        //        errors.Add("RCPT023: Invoice line quantity must be positive");
        //    }
        //}

        // RCPT025: Invalid tax is used
        // Specific to tax validation, implementation depends on available tax information

        // RCPT026: Incorrectly calculated tax amount
        foreach (var tax in receipt.ReceiptTaxes)
        {
            var expectedTaxAmount = (tax.TaxPercent.HasValue ? tax.TaxPercent.Value / 100 : 0) * tax.SalesAmountWithTax;
            if (Math.Abs(tax.TaxAmount - expectedTaxAmount) > 0.01)
            {
                errors.Add("RCPT026: Incorrectly calculated tax amount");
            }
        }

        return errors;
    }
}
