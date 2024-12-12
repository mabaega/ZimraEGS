using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Net;
using System.Text;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;
using ZimraEGS.Helpers;
using ZimraEGS.Models;


namespace ZimraEGS.Controllers
{
    public partial class SetupController : Controller
    {
        private readonly HttpClient _httpClient = new();

        public IActionResult Index()
        {
            var model = new SetupViewModel();
            return View(model);
        }

        public IActionResult Register()
        {
            SetupViewModel model = null;

            // Retrieve the model from TempData
            if (TempData["SetupViewModel"] != null)
            {
                model = JsonConvert.DeserializeObject<SetupViewModel>(TempData["SetupViewModel"].ToString());
            }

            // If the model is null, you might want to handle that case
            if (model == null)
            {
                // Handle the case where the model is not available
                // For example, you could redirect to the Index action
                return RedirectToAction("Index");
            }

            // If you want to return the Index view instead of Register
            return View("Index", model); // This will return the Index view with the model
        }


        [HttpGet("setup/GetCfData")]
        public IActionResult CustomFieldJson()
        {
            try
            {
                // Nama file: cfData.json
                string jsonData = Utils.ReadEmbeddedResource("cfData.json");
                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to load resource: {ex.Message}");
            }
        }

        [HttpPost("setup/verifytaxpayer")]
        public async Task<IActionResult> VerifyTaxPayerInformation([FromBody] VerifyTaxPayerDto verifyTaxPayerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Errors = "Invalid input data" });
            }

            try
            {
                VerifyTaxpayerInformationRequest verifyTaxpayerInformationRequest = new VerifyTaxpayerInformationRequest
                {
                    ActivationKey = verifyTaxPayerDto.ActivationKey,
                    DeviceSerialNo = verifyTaxPayerDto.DeviceSerialNumber
                };

                PlatformType integrationType = (PlatformType)Enum.Parse(typeof(PlatformType), verifyTaxPayerDto.IntegrationType);
                ApiHelper apiHelper = new ApiHelper(integrationType);
                ServerResponse serverResponse = await apiHelper.VerifyTaxpayerInformationAsync(
                    verifyTaxPayerDto.DeviceID,
                    verifyTaxpayerInformationRequest
                );

                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(serverResponse.ResponseContent);
                }

                return BadRequest(new { Errors = serverResponse.GetContentAsString() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Internal server error: {ex.Message}" });
            }
        }

        public string AddPemHeaderFooterToCsr(string base64Csr)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN CERTIFICATE REQUEST-----");
            sb.AppendLine(base64Csr);
            sb.AppendLine("-----END CERTIFICATE REQUEST-----");
            return sb.ToString();
        }


        [HttpPost("setup/registerdevice")]
        public async Task<IActionResult> RegisterDeviceAsync([FromBody] DeviceRegistrationDto deviceRegistration)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            if (deviceRegistration == null || string.IsNullOrEmpty(deviceRegistration.CommonName))
            {
                return BadRequest(new { Errors = "Common name is required." });
            }

            try
            {
                string errMessage = string.Empty;

                var (generatedCsr, privateKey) = RSA_CryptoHelper.GenerateCSR(deviceRegistration.CommonName);

                // Register Device
                RegisterDeviceRequest registerDeviceRequest = new RegisterDeviceRequest
                {
                    ActivationKey = deviceRegistration.ActivationKey,
                    CertificateRequest = generatedCsr
                };

                PlatformType integrationType = (PlatformType)Enum.Parse(typeof(PlatformType), deviceRegistration.IntegrationType);

                ApiHelper apiHelper = new ApiHelper(PlatformType.Simulation);
                ServerResponse serverResponse = await apiHelper.RegisterDeviceAsync(deviceRegistration.DeviceID, deviceRegistration.DeviceModelName, deviceRegistration.DeviceModelVersion, registerDeviceRequest);

                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    var deviceCertificate = serverResponse.GetContentAs<RegisterDeviceResponse>().Certificate;

                    string pfxbyte = Utilities.GeneratePfx(Utilities.CleanBase64String(privateKey), Utilities.CleanBase64String(deviceCertificate), (string)null);

                    var response = new
                    {
                        GeneratedCsr = Utilities.CleanBase64String(generatedCsr),
                        PrivateKey = Utilities.CleanBase64String(privateKey),
                        DeviceCertificate = Utilities.CleanBase64String(deviceCertificate),
                        Base64Pfx = pfxbyte
                    };

                    return Ok(response);
                }
                else
                {
                    errMessage = serverResponse.GetFullResponseAsJson();
                }

                return BadRequest(new { Errors = errMessage });
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return BadRequest(new { Errors = "Error generating CSR: " + ex.Message });
            }
        }


        [HttpPost("setup/getconfig")]
        public async Task<IActionResult> GetDeviceConfig([FromBody] GetConfigDto getConfigDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }

            if (getConfigDTO == null || string.IsNullOrEmpty(getConfigDTO.Base64Pfx))
            {
                return BadRequest(new { Errors = "Base64 PFX is required." });
            }

            try
            {
                string errMessage = string.Empty;

                PlatformType integrationType = (PlatformType)Enum.Parse(typeof(PlatformType), getConfigDTO.IntegrationType);

                // Create ApiHelper with the Base64 PFX string
                ApiHelper apiHelper = new ApiHelper(getConfigDTO.Base64Pfx, PlatformType.Simulation);

                ServerResponse serverResponse = await apiHelper.GetConfigAsync(getConfigDTO.DeviceID, getConfigDTO.DeviceModelName, getConfigDTO.DeviceModelVersion);

                if (serverResponse.StatusCode == HttpStatusCode.OK)
                {
                    GetConfigResponse getConfigResponse = serverResponse.GetContentAs<GetConfigResponse>();

                    var response = new
                    {
                        DeviceOperatingMode = getConfigResponse.DeviceOperatingMode,
                        ApplicableTaxes = string.Join("#", getConfigResponse.ApplicableTaxes.Select(x => $"{x.TaxID} - {x.TaxName} - {x.TaxPercent}")),
                        TaxPayerDayMaxHrs = getConfigResponse.TaxPayerDayMaxHrs,
                        TaxpayerDayEndNotificationHrs = getConfigResponse.TaxpayerDayEndNotificationHrs,
                        CertificateValidTill = getConfigResponse.CertificateValidTill,
                        QrUrl = getConfigResponse.QrUrl
                    };

                    return Ok(response);
                }
                else
                {
                    errMessage = serverResponse.GetContentAsString();
                }

                Console.WriteLine(errMessage);

                return BadRequest(new { Errors = errMessage });
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return BadRequest(new { Errors = "Error Get Device Configuration: " + ex.Message });
            }
        }


        [HttpPost("setup/savecertificate")]
        public IActionResult SaveCertificate(SetupViewModel viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(new { Errors = "Error Saving Certificate" });
            //}
            try
            {
                CertificateInfo certificateInfo = viewModel.CertificateInfo;
                var base64CertificateInfo = ObjectCompressor.SerializeToBase64String(certificateInfo);

                // Update businessDetails
                var businessDetails = viewModel.BusinessDetails;
                if (string.IsNullOrEmpty(viewModel.BusinessDetails))
                {
                    businessDetails = @"
                    {
                        ""Name"": """",
                        ""Address"": """",
                        ""CustomFields2"": {
                            ""Decimals"": {
                                ""8d99fa20-d203-4b83-b8f4-96e95bacb930"": 0,
                                ""b3d3836f-e798-4b5d-b5ba-4472ba62ebfb"": 0,
                                ""bc92c626-80ce-46da-bf97-189f047963a9"": 0,
                                ""f07244b7-314f-4d3a-b60d-dbe66bd9f3ef"": 0
                            },
                            ""Strings"": {
                                ""2147d331-3237-46b3-a842-0a5bff077f9d"": """",
                                ""65016233-ef27-43da-8dc7-5d808093faca"": """",
                                ""98eef11a-e241-4713-b73d-8219fce8b032"": """",
                                ""bf449d08-a4d9-4c8b-ac16-1ba366688d13"": """"
                            }
                        },
                        ""Name"": ""Business Name""
                    }";
                }

                businessDetails = RelayDataHelper.UpdateOrCreateField(businessDetails, "Name", certificateInfo.TaxPayerName);
                businessDetails = RelayDataHelper.UpdateOrCreateField(businessDetails, "Address", certificateInfo.ToBusinessAddress());
                businessDetails = RelayDataHelper.ModifyStringCustomFields2(businessDetails, ManagerCustomField.CertificateInfoGuid, base64CertificateInfo);
                //businessDetails = RelayDataHelper.UpdateOrCreateField(businessDetails, $"CustomFields2.Decimals.{ManagerCustomField.FiscalDayOpenedGuid}", 9090);
                viewModel.BusinessDetails = businessDetails;

                // Serialize BusinessDetails for JavaScript update
                viewModel.BusinessDetailsJson = JsonConvert.SerializeObject(businessDetails);

                // Console.WriteLine(viewModel.BusinessDetails);
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        //// File 1: Certificate Request
                        //var csrEntry = archive.CreateEntry("TaxPayer.csr");
                        //using (var writer = new StreamWriter(csrEntry.Open()))
                        //{
                        //    writer.Write(certificateInfo.GeneratedCSR);
                        //}

                        //// File 2: PrivateKey
                        //var privateKeyEntry = archive.CreateEntry("PrivateKey.pem");
                        //using (var writer = new StreamWriter(privateKeyEntry.Open()))
                        //{
                        //    writer.Write(certificateInfo.PrivateKey);
                        //}

                        //// File 3: Device Certificate
                        //var certificateEntry = archive.CreateEntry("DeviceCertificate.pem");
                        //using (var writer = new StreamWriter(certificateEntry.Open()))
                        //{
                        //    writer.Write(certificateInfo.DeviceCertificate);
                        //}

                        //// File 4: Cerver Certificate
                        //var serverCertificateEntry = archive.CreateEntry("SerVerCertificate.pem");
                        //using (var writer = new StreamWriter(certificateEntry.Open()))
                        //{
                        //    writer.Write(certificateInfo.ServerCertificate.First());
                        //}

                        // File 5: Integration info
                        var infoEntry = archive.CreateEntry($"{certificateInfo.VatNumber}_{certificateInfo.DeviceID.ToString().PadLeft(10, '0')}_{certificateInfo.IntegrationType.ToString()}.json");

                        using (var writer = new StreamWriter(infoEntry.Open()))
                        {
                            var infoJson = JsonConvert.SerializeObject(certificateInfo, Newtonsoft.Json.Formatting.Indented);
                            writer.Write(infoJson);
                        }
                    }

                    memoryStream.Position = 0;
                    var fileContent = memoryStream.ToArray();
                    viewModel.FileContent = Convert.ToBase64String(fileContent); // Convert file content to Base64
                    viewModel.Filename = $"{certificateInfo.VatNumber}_{certificateInfo.DeviceID.ToString().PadLeft(10, '0')}_{certificateInfo.IntegrationType.ToString()}.zip";
                    viewModel.IsFileReady = true;
                }

            }
            catch
            {

            }

            return View("index", viewModel);
        }
    }
}