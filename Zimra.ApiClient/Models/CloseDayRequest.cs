namespace Zimra.ApiClient.Models
{
    using System = System;

    public class CloseDayRequest
    {
        /// <summary>
        /// Fiscal day number.
        /// <br/>Validation rules:
        /// <br/>- fiscalDayNo must be the same as provided/received fiscalDayNo value in openDay request.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("fiscalDayNo", Required = Newtonsoft.Json.Required.Always)]
        public int FiscalDayNo { get; set; }

        /// <summary>
        /// List of fiscal counters.
        /// <br/>Zero value counters may not be submitted to Fiscalisation Backend.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("fiscalDayCounters", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public ICollection<FiscalDayCounter> FiscalDayCounters { get; set; } = new System.Collections.ObjectModel.Collection<FiscalDayCounter>();

        /// <summary>
        /// SignatureData structure with SHA256 hash of fiscal day report fields (hash used for signature) and fiscal day report device signature prepared by using device private key.
        /// <br/>Validation rules:
        /// <br/>- fiscalDayDeviceSignature must be valid
        /// </summary>
        [Newtonsoft.Json.JsonProperty("fiscalDayDeviceSignature", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public SignatureData FiscalDayDeviceSignature { get; set; } = new SignatureData();

        /// <summary>
        /// ReceiptCounter value of last receipt of current fiscal day.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("receiptCounter", Required = Newtonsoft.Json.Required.Always)]
        public int ReceiptCounter { get; set; }

    }

}
