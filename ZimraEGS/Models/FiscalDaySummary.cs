using ZimraEGS.ApiClient.Enums;

namespace ZimraEGS.Models
{
    public class FiscalDaySummary
    {
        public List<TaxSummary> TaxSummaries { get; set; }
        public List<PaymentSummary> PaymentSummaries { get; set; }
    }

    public class TaxSummary
    {
        public int FiscalDayNo { get; set; } = 0;
        public ReceiptType ReceiptType { get; set; }
        public string ReceiptCurrency { get; set; } = "ZWG";
        public string TaxCode { get; set; } = string.Empty;
        public string? TaxPersen { get; set; }
        public Double TaxAmount { get; set; } = 0;
        public Double SalesAmountWithTax { get; set; } = 0;
        public Double Amount => SalesAmountWithTax - TaxAmount;
    }
    public class PaymentSummary
    {
        public int FiscalDayNo { get; set; } = 0;
        public string ReceiptCurrency { get; set; } = "ZWG";
        public MoneyType MoneyTypeCode { get; set; } = MoneyType.Other;
        public Double PaymentAmount { get; set; } = 0;
    }
}
