namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Enums;
    using System = System;

    public class Receipt
    {
        [Newtonsoft.Json.JsonProperty("receiptType", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ReceiptType ReceiptType { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptCurrency", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(3)]
        public string ReceiptCurrency { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptCounter", Required = Newtonsoft.Json.Required.Always)]
        public int ReceiptCounter { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptGlobalNo", Required = Newtonsoft.Json.Required.Always)]
        public int ReceiptGlobalNo { get; set; }

        [Newtonsoft.Json.JsonProperty("invoiceNo", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string InvoiceNo { get; set; }

        [Newtonsoft.Json.JsonProperty("buyerData", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public Buyer BuyerData { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptNotes", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ReceiptNotes { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptDate", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [Newtonsoft.Json.JsonConverter(typeof(ZimraEGS.ApiClient.Helpers.LocalDateTimeConverter))]
        public DateTimeOffset ReceiptDate { get; set; }

        [Newtonsoft.Json.JsonProperty("creditDebitNote", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public CreditDebitNote CreditDebitNote { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLinesTaxInclusive", Required = Newtonsoft.Json.Required.Always)]
        public bool ReceiptLinesTaxInclusive { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptLines", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public ICollection<ReceiptLine> ReceiptLines { get; set; } = new System.Collections.ObjectModel.Collection<ReceiptLine>();

        [Newtonsoft.Json.JsonProperty("receiptTaxes", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public ICollection<ReceiptTax> ReceiptTaxes { get; set; } = new System.Collections.ObjectModel.Collection<ReceiptTax>();

        [Newtonsoft.Json.JsonProperty("receiptPayments", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public ICollection<Payment> ReceiptPayments { get; set; } = new System.Collections.ObjectModel.Collection<Payment>();

        [Newtonsoft.Json.JsonProperty("receiptTotal", Required = Newtonsoft.Json.Required.Always)]
        public double ReceiptTotal { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptPrintForm", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ReceiptPrintForm? ReceiptPrintForm { get; set; }

        [Newtonsoft.Json.JsonProperty("receiptDeviceSignature", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public SignatureData ReceiptDeviceSignature { get; set; } = new SignatureData();

    }

}
