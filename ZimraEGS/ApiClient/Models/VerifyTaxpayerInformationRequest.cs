namespace ZimraEGS.ApiClient.Models
{
    using System = System;

    public class VerifyTaxpayerInformationRequest
    {
        /// <summary>
        /// Case insensitive 8 symbols key
        /// </summary>
        [Newtonsoft.Json.JsonProperty("activationKey", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(8, MinimumLength = 1)]
        public string ActivationKey { get; set; }

        /// <summary>
        /// Device serial number assigned by manufacturer.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("deviceSerialNo", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(20, MinimumLength = 1)]
        public string DeviceSerialNo { get; set; }

    }


}
