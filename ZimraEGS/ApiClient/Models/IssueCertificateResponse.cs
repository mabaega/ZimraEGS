namespace ZimraEGS.ApiClient.Models
{
    using System = System;

    public class IssueCertificateResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        [Newtonsoft.Json.JsonProperty("certificate", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public string Certificate { get; set; }

    }

}
