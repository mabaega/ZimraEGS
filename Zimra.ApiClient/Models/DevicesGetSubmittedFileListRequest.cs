namespace Zimra.ApiClient.Models
{
    using Zimra.ApiClient.Enums;
    using Zimra.ApiClient.Helpers;
    using System = System;

    public class DevicesGetSubmittedFileListRequest
    {
        [Newtonsoft.Json.JsonProperty("order", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ListRequestOrderEnum Order { get; set; }

        [Newtonsoft.Json.JsonProperty("offset", Required = Newtonsoft.Json.Required.Always)]
        public int Offset { get; set; }

        [Newtonsoft.Json.JsonProperty("limit", Required = Newtonsoft.Json.Required.Always)]
        public int Limit { get; set; }

        [Newtonsoft.Json.JsonProperty("operator", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public LogicalOperator Operator { get; set; }

        [Newtonsoft.Json.JsonProperty("sort", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ListSubmittedFileEnum Sort { get; set; }

        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        [Newtonsoft.Json.JsonProperty("fileUploadedFrom", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset FileUploadedFrom { get; set; }

        [Newtonsoft.Json.JsonProperty("fileUploadedTill", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset FileUploadedTill { get; set; }

    }

}
