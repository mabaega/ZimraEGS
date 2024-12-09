namespace Zimra.ApiClient.Models
{
    using System = System;

    public class SendSecurityCodeToUserEmailRequest
    {
        [Newtonsoft.Json.JsonProperty("userEmail", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 1)]
        public string UserEmail { get; set; }

    }


}