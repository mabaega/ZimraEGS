namespace ZimraEGS.ApiClient.Models
{
    using System = System;

    public class ReceiptTax
    {
        [Newtonsoft.Json.JsonProperty("taxCode", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(3)]
        public string TaxCode { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPercent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? TaxPercent { get; set; }

        [Newtonsoft.Json.JsonProperty("taxID", Required = Newtonsoft.Json.Required.Always)]
        public int TaxID { get; set; }

        [Newtonsoft.Json.JsonProperty("taxAmount", Required = Newtonsoft.Json.Required.Always)]
        public double TaxAmount { get; set; }

        [Newtonsoft.Json.JsonProperty("salesAmountWithTax", Required = Newtonsoft.Json.Required.Always)]
        public double SalesAmountWithTax { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string TaxPersenString => TaxPercent.HasValue
        ? TaxPercent.Value.ToString("F2")
        : string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public int LineNumber { get; set; }

    }

}
