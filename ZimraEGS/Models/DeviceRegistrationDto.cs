using Zimra.ApiClient.Models;

namespace ZimraEGS.Controllers
{
    public partial class SetupController
    {
        public class DeviceRegistrationDto
        {
            public string IntegrationType { get; set; }
            public int DeviceID { get; set; }
            public string DeviceSerialNumber { get; set; }
            public string CommonName { get; set; }
            public string DeviceModelName { get; set; }
            public string DeviceModelVersion { get; set; }
            public string ActivationKey { get; set; }
        }

        public class VerifyTaxPayerDto
        {
            public string IntegrationType { get; set; } 
            public int DeviceID { get; set; } 
            public string DeviceSerialNumber { get; set; } 
            public string ActivationKey { get; set; } 
        }

        public class GetConfigDto
        {
            public string IntegrationType { get; set; }
            public int DeviceID { get; set; }
            public string DeviceSerialNumber { get; set; }
            public string DeviceModelName { get; set; }
            public string DeviceModelVersion { get; set; }
            public string DeviceCertificate { get; set; }
            public string PrivateKey { get; set; }
            public string Base64Pfx { get; set; }

        }

    }
}