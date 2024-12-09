namespace Zimra.ApiClient.Models
{
    using System = System;

    public class PingResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        /// <summary>
        /// Reporting frequency in minutes.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("reportingFrequency", Required = Newtonsoft.Json.Required.Always)]
        public int ReportingFrequency { get; set; }

    }

}
