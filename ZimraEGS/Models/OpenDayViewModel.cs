using Zimra.ApiClient.Enums;
using Zimra.ApiClient.Models;

namespace ZimraEGS.Models
{
    public class OpenDayViewModel
    {
        public int DeviceID { get; set; }
        public string DeviceModelName { get; set; }
        public string DeviceModelVersion { get; set; }
        public string DeviceSerialNumber { get; set; }
        public int LastFiscalDayNo { get; set; }
        public string Base64Pfx { get; set; }
        public EnvironmentType IntegrationType { get; set; }
        public string BusinessDetailJson { get; set; }
        public int LastReceiptCounter { get; set; }
        public string LastReceiptHash { get; set; }
        public FiscalDayStatus FiscalDayStatus { get; set; }
        public int LastReceiptGlobalNo { get; set; }
        public string DeviceStatusJson { get; set; }
        public string Api { get; set; }
        public string Token { get; set; }
        public string Referrer { get; set; }
        public OpenDayViewModel() { }

    }

    public class CloseDayViewModel
    {
        public string Api { get; set; }
        public string Token { get; set; }
        public string Referrer { get; set; }
        public string VerificationCode { get; set; }
        public string ReceiptQrCode{ get; set; }
        public InvoiceReference ReceiptReference { get; set; }
        public CertificateInfo CertificateInfo { get; set; }
        public string BusinessDetailJson { get; set; }
        public GetStatusResponse DeviceStatus { get; set; }
        public BusinessReference BusinessReference { get; set; }
        public CloseDayViewModel() { }
    }
}
