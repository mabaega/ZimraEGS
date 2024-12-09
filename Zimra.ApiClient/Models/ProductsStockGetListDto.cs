namespace Zimra.ApiClient.Models
{
    using System = System;

    public class ProductsStockGetListDto
    {
        [Newtonsoft.Json.JsonProperty("hsCode", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(8, MinimumLength = 1)]
        public string HsCode { get; set; }

        [Newtonsoft.Json.JsonProperty("goodName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(200, MinimumLength = 1)]
        public string GoodName { get; set; }

        [Newtonsoft.Json.JsonProperty("quantity", Required = Newtonsoft.Json.Required.Always)]
        public double Quantity { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerId", Required = Newtonsoft.Json.Required.Always)]
        public int TaxPayerId { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerTIN", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public string TaxPayerTIN { get; set; }

        [Newtonsoft.Json.JsonProperty("taxPayerName", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(250, MinimumLength = 1)]
        public string TaxPayerName { get; set; }

        [Newtonsoft.Json.JsonProperty("branchId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? BranchId { get; set; }

        [Newtonsoft.Json.JsonProperty("branchName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string BranchName { get; set; }

    }


}
