namespace ZimraEGS.ApiClient.Models
{
    using ZimraEGS.ApiClient.Helpers;
    using System = System;

    public class ValidationError
    {
        [Newtonsoft.Json.JsonProperty("validationErrorCode", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(10)]
        public string ValidationErrorCode { get; set; }

        [Newtonsoft.Json.JsonProperty("validationErrorColor", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(10)]
        public string ValidationErrorColor { get; set; }

        public string ValidationErrorText => ValidationText.GetErrorString(ValidationErrorCode);

    }

}
