namespace Zimra.ApiClient.Models
{
    using System = System;

    public class SubmitReceiptRequest
    {
        /// <summary>
        /// Receipt data
        /// </summary>
        [Newtonsoft.Json.JsonProperty("receipt", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public Receipt Receipt { get; set; } = new Receipt();

    }

}
