namespace Zimra.ApiClient.Models
{
    using Zimra.ApiClient.Enums;
    using System = System;

    public class ConfirmContactRequest
    {
        [Newtonsoft.Json.JsonProperty("securityCode", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(10, MinimumLength = 1)]
        public string SecurityCode { get; set; }

        [Newtonsoft.Json.JsonProperty("channel", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public SendSecurityCodeToEnum Channel { get; set; }

    }


}