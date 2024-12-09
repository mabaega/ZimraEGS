namespace Zimra.ApiClient.Models
{
    using System = System;

    public class RegisterDeviceRequest
    {
        /// <summary>
        /// Certificate signing request (CSR) for which certificate will be generated (in PEM format).
        /// <br/>Assigned by ZIMRA device name (format: ZIMRA-[Fiscal_device_serial_no]-[zero_padded_10_digit_deviceId]) should be provided in CSR`s Subject CN.
        /// <br/>Supported algorithms and key types (in order of suggested preference):
        /// <br/>1)	ECC ECDSA on SECG secp256r1 curve (also named as ANSI prime256v1, NIST P-256); Signature Algorithm: ecdsa-with-SHA256.
        /// <br/>2)	RSA 2048; Signature Algorithm - SHA256WithRSA.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("certificateRequest", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public string CertificateRequest { get; set; }

        /// <summary>
        /// Case insensitive 8 symbols key
        /// </summary>
        [Newtonsoft.Json.JsonProperty("activationKey", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        [System.ComponentModel.DataAnnotations.StringLength(8)]
        public string ActivationKey { get; set; }

    }


}
