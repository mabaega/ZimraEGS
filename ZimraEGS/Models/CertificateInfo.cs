using Newtonsoft.Json;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;

namespace ZimraEGS.Models
{
    public class CertificateInfo
    {
        public PlatformType IntegrationType { get; set; } = PlatformType.Simulation;
        public int DeviceID { get; set; } = 0;
        public string DeviceSerialNumber { get; set; } = "";
        public string ActivationKey { get; set; } = "";
        public string DeviceModelName { get; set; } = "Server";
        public string DeviceModelVersion { get; set; } = "v1";

        public string CommonName { get; set; } = string.Empty;
        public string TaxPayerName { get; set; } = string.Empty;
        public string TaxPayerTIN { get; set; } = "";
        public string VatNumber { get; set; } = "";
        public string DeviceBranchName { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HouseNo { get; set; } = string.Empty;

        public string PhoneNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string GeneratedCSR { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string DeviceCertificate { get; set; }
        public string Base64Pfx { get; set; } = string.Empty;

        public int DeviceOperatingMode { get; set; }
        public string ApplicableTaxes { get; set; }
        public int TaxPayerDayMaxHrs { get; set; }
        public int TaxpayerDayEndNotificationHrs { get; set; }
        public string QrUrl { get; set; }

        [JsonProperty("CertificateValidTill")]
        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset CertificateValidTill { get; set; }

        public string ApiBaseUrl => Utilities.GetBaseUrl(IntegrationType);
        public CertificateInfo()
        {
            DeviceID = 0;
            DeviceSerialNumber = "";
            DeviceModelName = "Server";
            DeviceModelVersion = "v1";
            ActivationKey = "";
        }
        public string ToBusinessAddress()
        {
            return $"<div>\n<div style='font-weight: bold; color: black; text-shadow: 2px 2px 4px #aaa;'>SELLER</div>\n<div style='font-weight: bold;'>{TaxPayerName}</div>\n<div>{DeviceBranchName}</div>\n<div>TIN: {TaxPayerTIN} — Vat No: {VatNumber}</div>\n<div>{Province} — {City} <br>{Street} — {HouseNo}</div>\n<div>Email: {Email}</div>\n<div>Phone No: {PhoneNo}</div>\n</div>";
        }
    }

    public class CertInfoViewModel
    {
        public string BusinessDetailJson { get; set; }
        public string Api { get; set; }
        public string Token { get; set; }
        public string Referrer { get; set; }
        public CertificateInfo CertificateInfo { get; set; }
        public CertInfoViewModel() { }
    }
}
