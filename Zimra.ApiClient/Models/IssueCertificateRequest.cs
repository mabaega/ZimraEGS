namespace Zimra.ApiClient.Models
{
    using System = System;

    public class IssueCertificateRequest
    {
        /// <summary>
        /// Certificate signing request (CSR) for which certificate will be generated (in PEM format).
        /// <br/>certificateRequest requirements are specified in registerDevice endpoint description.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("certificateRequest", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public string CertificateRequest { get; set; }

    }

}
