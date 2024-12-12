using Newtonsoft.Json;
using ZimraEGS.ApiClient.Enums;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;

namespace ZimraEGS.Models
{

    public class RelayDataViewModel
    {
        public string Referrer { get; set; } = string.Empty;
        public string FormKey { get; set; } = string.Empty;
        public string Api { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        public string InvoiceNumber { get; set; } = string.Empty;

        [JsonProperty("InvoiceDate")]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset InvoiceDate { get; set; }
        public double InvoiceTotal { get; set; } = 0;
        public string BuyerRegisterName { get; set; } = string.Empty;
        public double InvoiceRoundOff { get; set; } = 0;

        public int DeviceID { get; set; } = 0;
        public string DeviceSN { get; set; } = string.Empty;
        public string DeviceModelName { get; set; } = string.Empty;
        public string DeviceModelVersion { get; set; } = string.Empty;

        public PlatformType IntegrationType { get; set; } = PlatformType.Simulation;
        public string Base64Pfx { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;

        public FiscalDayStatus FiscalDayStatus { get; set; } = FiscalDayStatus.FiscalDayClosed;
        public int FiscalDayNo { get; set; } = 0;
        public int ReceiptGlobalNo { get; set; } = 0;
        public int ReceiptCounter { get; set; } = 0;

        public string FiscalDayOpened { get; set; } = string.Empty;
        public string FiscalDaySummaryJson { get; set; }

        public string ReceiptNotes { get; set; } = string.Empty;
        public int DeviceIDRef { get; set; } = 0;
        public string DeviceSNRef { get; set; } = string.Empty;
        public int FiscalDayNoRef { get; set; } = 0;
        public int ReceiptCounterRef { get; set; } = 0;
        public int ReceiptGlobalNoRef { get; set; } = 0;
        public int ReceiptRefNo { get; set; } = 0;
        public string ReceiptRefDate { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;
        public ReceiptType ReceiptType { get; set; } = ReceiptType.FiscalInvoice;

        public double TaxAmount { get; set; } = 0;
        public double AmountWithTax { get; set; } = 0;
        public double Amount => AmountWithTax - TaxAmount;

        public string ReceiptQrCode { get; set; }
        public string ReceiptHash { get; set; }
        public string ReceiptSignature { get; set; }
        public string ReceiptVerificationCode { get; set; }
        public string TmpReceiptVerificationCode { get; set; }

        public string InvoiceJson { get; set; }
        public string ReceiptJson { get; set; }

        public string BusinessDetailJson { get; set; } = string.Empty;
        public string InvoiceReferenceJson { get; set; }
        public string BusinessReferenceJson { get; set; }

        public bool TimeForCloseDay { get; set; }
        public RelayDataViewModel() { }

    }
}