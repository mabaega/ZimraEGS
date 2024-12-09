namespace Zimra.ApiClient.Models
{
    public class CreditDebitNote
    {
        [Newtonsoft.Json.JsonProperty("receiptID", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public long? ReceiptID { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceID", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? DeviceID { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptGlobalNo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? ReceiptGlobalNo { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayNo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? FiscalDayNo { get; set; }

    }

}
