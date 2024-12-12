namespace ZimraEGS.ApiClient.Models
{
    using System = System;

    public class SignatureDataEx
    {
        [Newtonsoft.Json.JsonProperty("hash", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(32)]
        public byte[] Hash { get; set; }

        [Newtonsoft.Json.JsonProperty("signature", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(256)]
        public byte[] Signature { get; set; }

        /// <summary>
        /// SHA-1 Thumbprint of Certificate used for signature
        /// </summary>
        [Newtonsoft.Json.JsonProperty("certificateThumbprint", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public string CertificateThumbprint { get; set; }

    }

}
