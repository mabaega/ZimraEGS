namespace Zimra.ApiClient.Models
{
    public class InvoiceWithValidationError
    {
        [Newtonsoft.Json.JsonProperty("receiptCounter", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? ReceiptCounter { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptGlobalNo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? ReceiptGlobalNo { get; set; }

        [Newtonsoft.Json.JsonProperty("validationErrors", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<ValidationError> ValidationErrors { get; set; }

    }

}
