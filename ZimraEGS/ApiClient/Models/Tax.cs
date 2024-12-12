namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Helpers;
    using System = System;

    public class Tax
    {
        [Newtonsoft.Json.JsonProperty("taxPercent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? TaxPercent { get; set; }

        [Newtonsoft.Json.JsonProperty("taxName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string TaxName { get; set; }

        [Newtonsoft.Json.JsonProperty("validFrom", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset ValidFrom { get; set; }

        [Newtonsoft.Json.JsonProperty("validTill", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset? ValidTill { get; set; }

        /// <summary>
        /// Tax ID uniquely identifying a tax. This tax ID must be used in submitting invoices.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("taxID", Required = Newtonsoft.Json.Required.Always)]
        public int TaxID { get; set; }

    }

}
