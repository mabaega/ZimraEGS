namespace Zimra.ApiClient.Models
{
    using System = System;

    public class AddressDto
    {
        public AddressDto()
        {
        }

        [Newtonsoft.Json.JsonProperty("province", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string Province { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonProperty("city", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonProperty("street", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonProperty("houseNo", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(250)]
        public string HouseNo { get; set; } = string.Empty;

        //[Newtonsoft.Json.JsonProperty("district", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        //[System.ComponentModel.DataAnnotations.StringLength(100)]
        //public string District { get; set; } = string.Empty;

    }

}
