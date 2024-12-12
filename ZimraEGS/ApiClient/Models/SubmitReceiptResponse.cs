namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Helpers;
    using System = System;

    public class SubmitReceiptResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptID", Required = Newtonsoft.Json.Required.Always)]
        public long ReceiptID { get; set; }

        [Newtonsoft.Json.JsonProperty("serverDate", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset ServerDate { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptServerSignature", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public SignatureDataEx ReceiptServerSignature { get; set; } = new SignatureDataEx();

        [Newtonsoft.Json.JsonProperty("validationErrors", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<ValidationError> ValidationErrors { get; set; }

    }

}
