namespace Zimra.ApiClient.Models
{
    using Zimra.ApiClient.Enums;
    using Zimra.ApiClient.Helpers;
    using System = System;

    public class SubmittedFileHeaderDto
    {
        [Newtonsoft.Json.JsonProperty("fileName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string FileName { get; set; }

        [Newtonsoft.Json.JsonProperty("fileUploadDate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset? FileUploadDate { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? DeviceID { get; set; }

        [Newtonsoft.Json.JsonProperty("dayNo", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int DayNo { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayOpenedAt", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset FiscalDayOpenedAt { get; set; }

        [Newtonsoft.Json.JsonProperty("fileSequence", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int FileSequence { get; set; }

        [Newtonsoft.Json.JsonProperty("fileProcessingDate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset? FileProcessingDate { get; set; }

        [Newtonsoft.Json.JsonProperty("fileProcessingStatus", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FileProcessingStatusEnum FileProcessingStatus { get; set; }

        [Newtonsoft.Json.JsonProperty("fileProcessingError", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FileProcessingErrorEnum? FileProcessingError { get; set; }

        [Newtonsoft.Json.JsonProperty("hasFooter", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public bool HasFooter { get; set; }

        [Newtonsoft.Json.JsonProperty("operationId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string OperationId { get; set; }

        [Newtonsoft.Json.JsonProperty("ipAddress", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string IpAddress { get; set; }

        [Newtonsoft.Json.JsonProperty("invoiceWithValidationErrors", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<InvoiceWithValidationError> InvoiceWithValidationErrors { get; set; }

    }

}
