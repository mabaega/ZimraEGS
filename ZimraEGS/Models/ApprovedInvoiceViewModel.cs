using ZimraEGS.ApiClient.Enums;

namespace ZimraEGS.Models
{
    public class ApprovedInvoiceViewModel
    {
        public string Referrer { get; set; }
        public string ReceiptQrCode { get; set; }
        public string ReceiptVerificationCode { get; set; }
        public string ReceiptReferenceJson { get; set; }

        public string Api { get; set; }
        public string Token { get; set; }
        public string BusinessDetailJson { get; set; }

        public bool TimeForCloseDay { get; set; }

        public FiscalDayStatus FiscalDayStatus { get; set; }

        public ApprovedInvoiceViewModel() { }
    }
}