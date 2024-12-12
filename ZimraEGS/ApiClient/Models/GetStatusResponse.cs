namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Enums;
    using ZimraEGS.ApiClient.Helpers;
    using System = System;

    public class GetStatusResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayStatus", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FiscalDayStatus FiscalDayStatus { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayReconciliationMode", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FiscalDayReconciliationMode? FiscalDayReconciliationMode { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayServerSignature", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public SignatureDataEx FiscalDayServerSignature { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayClosed", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset? FiscalDayClosed { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayCounter", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<FiscalDayCounter> FiscalDayCounter { get; set; }

        [Newtonsoft.Json.JsonProperty("lastReceiptGlobalNo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int LastReceiptGlobalNo { get; set; } = 0;

        [Newtonsoft.Json.JsonProperty("lastFiscalDayNo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? LastFiscalDayNo { get; set; }

        [Newtonsoft.Json.JsonProperty("fiscalDayClosingErrorCode", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public FiscalDayProcessingError? FiscalDayClosingErrorCode { get; set; }

        /// <summary>
        /// List of fiscal day document quantities. This field is returned only when fiscalDayStatus is �FiscalDayClosed� and fiscalDayReconciliationMode is �Manual�. FiscalDayDocumentQuantity type description provided in FiscalDayDocumentQuantity table.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("fiscalDayDocumentQuantities", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<FiscalDayDocumentQuantity> FiscalDayDocumentQuantities { get; set; }

    }

}
