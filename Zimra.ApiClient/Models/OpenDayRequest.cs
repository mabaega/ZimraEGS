namespace Zimra.ApiClient.Models
{
    using Zimra.ApiClient.Helpers;
    using System = System;

    public class OpenDayRequest
    {
        /// <summary>
        /// Fiscal day number assigned by device.
        /// <br/>If this field is not sent, Fiscalisation Backend will generate fiscal day number and return to device.
        /// <br/>Validation rules:
        /// <br/>- fiscalDayNo must be equal to 1 for the first fiscal day of fiscal device
        /// <br/>- fiscalDayNo must be greater by one from the last closed fiscal day fiscalDayNo.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("fiscalDayNo", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? FiscalDayNo { get; set; }

        /// <summary>
        /// Date and time when fiscal day was opened on a device. Time is provided in local time without time zone information.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("fiscalDayOpened", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset FiscalDayOpened { get; set; }

    }

}
