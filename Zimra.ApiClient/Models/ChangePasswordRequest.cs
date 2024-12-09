namespace Zimra.ApiClient.Models
{
    using System = System;

    public class ChangePasswordRequest
    {
        [Newtonsoft.Json.JsonProperty("oldPassword", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 1)]
        public string OldPassword { get; set; }

        [Newtonsoft.Json.JsonProperty("newPassword", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 1)]
        public string NewPassword { get; set; }

    }


}