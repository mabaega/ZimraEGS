namespace Zimra.ApiClient.Models
{
    using Zimra.ApiClient.Enums;
    using System = System;

    public class FiscalDayDocumentQuantity
    {
        /// <summary>
        /// Type of receipt
        /// </summary>
        [Newtonsoft.Json.JsonProperty("receiptType", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ReceiptType ReceiptType { get; set; }

        /// <summary>
        /// Receipt counter currency (ISO 4217 currency code).
        /// </summary>
        [Newtonsoft.Json.JsonProperty("receiptCurrency", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(3, MinimumLength = 3)]
        public string ReceiptCurrency { get; set; }

        /// <summary>
        /// Total quantity of receipts of particular receipt type and currency for fiscal day.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("receiptQuantity", Required = Newtonsoft.Json.Required.Always)]
        public int ReceiptQuantity { get; set; }

        /// <summary>
        /// Total receipt amount (including tax) of receipts of particular receipt type and currency for fiscal day.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("receiptTotalAmount", Required = Newtonsoft.Json.Required.Always)]
        public double ReceiptTotalAmount { get; set; }

    }

}
