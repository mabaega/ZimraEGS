using Newtonsoft.Json;
using ZimraEGS.ApiClient.Enums;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;

namespace ZimraEGS.Models
{
    public class BusinessReference
    {
        public PlatformType IntegrationType { get; set; } = PlatformType.Simulation;
        public int DeviceID { get; set; } = 0;
        public string DeviceSerialNumber { get; set; } = string.Empty;
        public FiscalDayStatus FiscalDayStatus { get; set; } = FiscalDayStatus.FiscalDayClosed;
        
        [JsonProperty("FiscalDayOpened")]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset FiscalDayOpened { get; set; }

        public int LastReceiptGlobalNo { get; set; } = 0;
        public int LastFiscalDayNo { get; set; } = 0;
        public int LastReceiptCounter { get; set; } = 0;
        public string ReceiptUUID { get; set; } = string.Empty;
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
