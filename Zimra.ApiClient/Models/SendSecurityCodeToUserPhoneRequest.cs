namespace Zimra.ApiClient.Models
{
    using System = System;

    public class SendSecurityCodeToUserPhoneRequest
    {
        [Newtonsoft.Json.JsonProperty("phoneNo", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(20, MinimumLength = 1)]
        public string PhoneNo { get; set; }

    }


}