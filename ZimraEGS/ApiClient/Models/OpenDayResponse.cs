namespace ZimraEGS.ApiClient.Models
{
    using System = System;

    public class OpenDayResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayNo", Required = Newtonsoft.Json.Required.Always)]
        public int FiscalDayNo { get; set; }

    }

}
