namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Enums;
    using System = System;

    public class FiscalDayCounter
    {
        [Newtonsoft.Json.JsonProperty("fiscalCounterType", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FiscalCounterType FiscalCounterType { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalCounterCurrency", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(3, MinimumLength = 3)]
        public string FiscalCounterCurrency { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalCounterTaxPercent", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? FiscalCounterTaxPercent { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalCounterTaxID", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? FiscalCounterTaxID { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalCounterMoneyType", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public MoneyType? FiscalCounterMoneyType { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalCounterValue", Required = Newtonsoft.Json.Required.Always)]
        public double FiscalCounterValue { get; set; }

    }

}
