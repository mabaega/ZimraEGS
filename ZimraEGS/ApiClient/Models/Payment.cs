namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Enums;
    using System = System;

    public class Payment
    {
        [Newtonsoft.Json.JsonProperty("moneyTypeCode", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public MoneyType MoneyTypeCode { get; set; }

        [Newtonsoft.Json.JsonProperty("paymentAmount", Required = Newtonsoft.Json.Required.Always)]
        public double PaymentAmount { get; set; }

    }

}
