namespace ZimraEGS.ApiClient.Models
{
    using System = System;

    public class SignatureData
    {
        [Newtonsoft.Json.JsonProperty("hash", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(32)]
        public byte[] Hash { get; set; }

        [Newtonsoft.Json.JsonProperty("signature", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(256)]
        public byte[] Signature { get; set; }

    }

}
