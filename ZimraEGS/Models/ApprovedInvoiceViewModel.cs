namespace ZimraEGS.Models
{
    public class ApprovedInvoiceViewModel
    {
        public string Referrer { get; set; }
        public string ReceiptQrCode { get; set; }
        public string ReceiptVerificationCode { get; set; }
        public string ReceiptReferenceJson { get; set; }

        public ApprovedInvoiceViewModel() { }
    }
}