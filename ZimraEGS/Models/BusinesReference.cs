using Zimra.ApiClient.Enums;
using Zimra.ApiClient.Models;

namespace ZimraEGS.Models
{
    public class BusinessReference
    {
        public EnvironmentType IntegrationType { get; set; } = EnvironmentType.Simulation;
        public int DeviceID { get; set; } = 0;
        public string DeviceSerialNumber { get; set; } = string.Empty;
        public FiscalDayStatus FiscalDayStatus { get; set; } = FiscalDayStatus.FiscalDayClosed;
        public string FiscalDayOpened { get; set; } = string.Empty;
        public int LastReceiptGlobalNo { get; set; } = 0;
        public int LastFiscalDayNo { get; set; } = 0;
        public int LastReceiptCounter { get; set; } = 0;
        public string LastApprovalStatus { get; set; } = string.Empty;
        public string LastReceiptHash { get; set; } = string.Empty;
        public BusinessReference() { }
    }

    public class InvoiceReference
    {
        public string ApprovalStatus { get; set; }
        public string ReceiptHash { get; set; }
        public string ReceiptSignature { get; set; }
        public SubmitReceiptResponse SubmitReceiptResponse { get; set; }
        public InvoiceReference() { }

    }
}
