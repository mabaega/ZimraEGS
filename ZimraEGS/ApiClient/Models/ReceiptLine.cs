namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Enums;
    using System = System;

    public class ReceiptLine
    {
        [Newtonsoft.Json.JsonProperty("receiptLineType", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ReceiptLineType ReceiptLineType { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLineNo", Required = Newtonsoft.Json.Required.Always)]
        public int ReceiptLineNo { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLineHSCode", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(8)]
        public string ReceiptLineHSCode { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLineName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(200)]
        public string ReceiptLineName { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLinePrice", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? ReceiptLinePrice { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLineQuantity", Required = Newtonsoft.Json.Required.Always)]
        public double ReceiptLineQuantity { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLineTotal", Required = Newtonsoft.Json.Required.Always)]
        public double ReceiptLineTotal { get; set; }

        //[Newtonsoft.Json.JsonProperty("taxCode", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        //[System.ComponentModel.DataAnnotations.StringLength(3)]
        //public string TaxCode { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPercent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? TaxPercent { get; set; }

        [Newtonsoft.Json.JsonProperty("taxID", Required = Newtonsoft.Json.Required.Always)]
        public int TaxID { get; set; }

    }

}
