namespace Zimra.ApiClient.Models
{
    public class SubmittedFileHeaderDtoListResponse
    {
        [Newtonsoft.Json.JsonProperty("total", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Total { get; set; }

        [Newtonsoft.Json.JsonProperty("rows", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ICollection<SubmittedFileHeaderDto> Rows { get; set; }

    }

}
