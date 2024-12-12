using ZimraEGS.ApiClient.Enums;
using ZimraEGS.ApiClient.Models;

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
        public PlatformType IntegrationType { get; set; }
        public string BusinessDetailJson { get; set; }
        public int LastReceiptCounter { get; set; }
        public string LastReceiptHash { get; set; } = string.Empty;
        public FiscalDayStatus FiscalDayStatus { get; set; }
        public int LastReceiptGlobalNo { get; set; }
        public string DeviceStatusJson { get; set; }
        public string Api { get; set; }
        public string Token { get; set; }
        public string Referrer { get; set; }
        public OpenDayViewModel() { }

    }

}
