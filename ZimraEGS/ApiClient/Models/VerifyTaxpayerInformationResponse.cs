namespace ZimraEGS.ApiClient.Models
{
    using System = System;

    public class VerifyTaxpayerInformationResponse
    {
        /// <summary>
        /// Operation ID assigned by Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("operationID", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(60)]
        public string OperationID { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(250, MinimumLength = 1)]
        public string TaxPayerName { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerTIN", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(10, MinimumLength = 1)]
        public string TaxPayerTIN { get; set; }

        [Newtonsoft.Json.JsonProperty("vatNumber", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [System.ComponentModel.DataAnnotations.StringLength(9)]
        public string VatNumber { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceBranchName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(250, MinimumLength = 1)]
        public string DeviceBranchName { get; set; }

        [Newtonsoft.Json.JsonProperty("deviceBranchAddress", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public AddressDto DeviceBranchAddress { get; set; } = new AddressDto();

        [Newtonsoft.Json.JsonProperty("deviceBranchContacts", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Contacts DeviceBranchContacts { get; set; }

    }


}
