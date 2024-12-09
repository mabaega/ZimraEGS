namespace Zimra.ApiClient.Models
{
    using System = System;

    public class Buyer
    {
        [Newtonsoft.Json.JsonProperty("buyerRegisterName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(200)]
        public required string BuyerRegisterName { get; set; }

        [Newtonsoft.Json.JsonProperty("buyerTradeName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(200)]
        public required string BuyerTradeName { get; set; }

        [Newtonsoft.Json.JsonProperty("vatNumber", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(9, MinimumLength = 9)]
        public required string VatNumber { get; set; }

        [Newtonsoft.Json.JsonProperty("buyerTIN", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(10, MinimumLength = 10)]
        public required string BuyerTIN { get; set; }

        [Newtonsoft.Json.JsonProperty("buyerContacts", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public required Contacts BuyerContacts { get; set; }

        [Newtonsoft.Json.JsonProperty("buyerAddress", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public required AddressDto BuyerAddress { get; set; }

    }

}
