namespace Zimra.ApiClient.Models
{
    using Zimra.ApiClient.Enums;
    using Zimra.ApiClient.Helpers;
    using System = System;

    public class GetConfigResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(250)]
        public string TaxPayerName { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerTIN", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(10, MinimumLength = 10)]
        public string TaxPayerTIN { get; set; }

        [Newtonsoft.Json.JsonProperty("vatNumber", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(9, MinimumLength = 9)]
        public string VatNumber { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceSerialNo", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(20)]
        public string DeviceSerialNo { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceBranchName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(250)]
        public string DeviceBranchName { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceBranchAddress", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public AddressDto DeviceBranchAddress { get; set; } = new AddressDto();

        [Newtonsoft.Json.JsonProperty("deviceBranchContacts", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Contacts DeviceBranchContacts { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceOperatingMode", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public DeviceOperatingMode DeviceOperatingMode { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerDayMaxHrs", Required = Newtonsoft.Json.Required.Always)]
        public int TaxPayerDayMaxHrs { get; set; }

        [Newtonsoft.Json.JsonProperty("applicableTaxes", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public ICollection<Tax> ApplicableTaxes { get; set; } = new System.Collections.ObjectModel.Collection<Tax>();

        [Newtonsoft.Json.JsonProperty("certificateValidTill", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset CertificateValidTill { get; set; }

        [Newtonsoft.Json.JsonProperty("qrUrl", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string QrUrl { get; set; }

        /// <summary>
        /// How much time in hours before end of fiscal day device should show notification to salesperson.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("taxpayerDayEndNotificationHrs", Required = Newtonsoft.Json.Required.Always)]
        public int TaxpayerDayEndNotificationHrs { get; set; }

    }

}
