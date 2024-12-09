namespace Zimra.ApiClient.Models
{
    using System = System;

    public class ConfirmPasswordResetRequest
    {
        [Newtonsoft.Json.JsonProperty("userName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 1)]
        public string UserName { get; set; }

        [Newtonsoft.Json.JsonProperty("securityCode", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(10, MinimumLength = 1)]
        public string SecurityCode { get; set; }

        [Newtonsoft.Json.JsonProperty("newPassword", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 1)]
        public string NewPassword { get; set; }

    }


}