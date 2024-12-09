namespace Zimra.ApiClient.Models
{
    using System = System;

    public class SendSecurityCodeToTaxpayerRequest
    {
        [Newtonsoft.Json.JsonProperty("userName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 1)]
        public string UserName { get; set; }

    }


}