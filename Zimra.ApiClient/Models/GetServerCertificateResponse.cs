namespace Zimra.ApiClient.Models
{
    using Zimra.ApiClient.Helpers;
    using System = System;

    public class GetServerCertificateResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        /// <summary>
        /// Fiscalisation Backend certificate chain (according x.509 standard) to validate Fiscalisation Backend signatures.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("certificate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<string> Certificate { get; set; }

        /// <summary>
        /// Date till when Fiscalisation Backend signing certificate is valid (despite that in the certificate parameter all the certificate chain is returned, this field shows validity time of the child certificate in the chain). Times is provided in UTC time.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("certificateValidTill", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset CertificateValidTill { get; set; }

    }


}
