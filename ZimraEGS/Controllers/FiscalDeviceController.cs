using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;
using ZimraEGS.Helpers;
using ZimraEGS.Models;

namespace ZimraEGS.Controllers
{
    public class FiscalDeviceController : Controller
    {
        public IActionResult Index()
        {
            var model = new RelayData();
            return View(model);
        }

        public IActionResult IssueCerificate()
        {
            // Retrieve the Base64 string from TempData
            if (TempData["CertInfoViewModel"] != null)
            {
                string certInfoJson = TempData["CertInfoViewModel"].ToString();
                CertInfoViewModel model = JsonConvert.DeserializeObject<CertInfoViewModel>(certInfoJson);
                return View(model);
            }
            // If TempData is empty, redirect to an error page
            return RedirectToAction("Error", "Home");
        }

        public IActionResult OpenDay()
        {
            // Retrieve the Base64 string from TempData
            if (TempData["OpenDayViewModel"] != null)
            {
                string openDayViewModel = TempData["OpenDayViewModel"].ToString();

                // Decode the Base64 string to get the serialized object
                OpenDayViewModel model = JsonConvert.DeserializeObject<OpenDayViewModel>(openDayViewModel);

                // Return the view with the deserialized relayData
                return View(model);
            }

            // If TempData is empty, redirect to an error page
            return RedirectToAction("Error", "Home");
        }

        public IActionResult CloseDay()
        {
            if (TempData["CloseDayViewModel"] is string relayDataJson)
            {
                string closeDayViewModel = TempData["CloseDayViewModel"].ToString();
                CloseDayViewModel model = JsonConvert.DeserializeObject<CloseDayViewModel>(closeDayViewModel);
                return View(model);
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AjaxOpenDay([FromForm] OpenDayViewModel model)
        {
            try
            {
                var deviceID = model.DeviceID;
                var deviceModelName = model.DeviceModelName;
                var deviceModelVersion = model.DeviceModelVersion;

                OpenDayRequest openDayRequest = new OpenDayRequest()
                {
                    FiscalDayNo = model.LastFiscalDayNo + 1,
                    FiscalDayOpened = DateTimeOffset.Now
                };

                ApiHelper apiHelper = new ApiHelper(model.Base64Pfx, model.IntegrationType);
                ServerResponse openDayResponse = await apiHelper.SendPostRequestAsync<OpenDayRequest>(
                    "OpenDay",
                    deviceID,
                    deviceModelName,
                    deviceModelVersion,
                    openDayRequest
                );

                if (openDayResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ServerResponse deviceStatusResponse = await apiHelper.SendGetRequestAsync(
                        "GetStatus",
                        deviceID,
                        deviceModelName,
                        deviceModelVersion
                    );

                    if (deviceStatusResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var deviceStatus = deviceStatusResponse.GetContentAs<GetStatusResponse>();

                        if (deviceStatus.FiscalDayStatus == ZimraEGS.ApiClient.Enums.FiscalDayStatus.FiscalDayClosed)
                        {
                            return StatusCode((int)deviceStatusResponse.StatusCode, new { Error = deviceStatusResponse.GetFullResponseAsJson() });
                        }

                        // Modify the business JSON with the new values
                        var businessJson = model.BusinessDetailJson;

                        // Modify with updated properties
                        businessJson = RelayDataHelper.ModifyStringCustomFields2(businessJson, ManagerCustomField.FiscalDaySummaryGuid, null);

                        // Create a new BusinessReference object and modify only the needed properties
                        var businessReference = new BusinessReference
                        {
                            LastReceiptCounter = model.LastReceiptCounter, // Use the updated value
                            LastReceiptHash = model.LastReceiptHash,       // Use the updated value
                            IntegrationType = model.IntegrationType,
                            DeviceID = model.DeviceID,
                            DeviceSerialNumber = model.DeviceSerialNumber,
                            FiscalDayOpened = openDayRequest.FiscalDayOpened,
                            FiscalDayStatus = model.FiscalDayStatus,
                            LastFiscalDayNo = model.LastFiscalDayNo,
                            LastReceiptGlobalNo = model.LastReceiptGlobalNo
                        };

                        // Modify the business JSON with the updated BusinessReference details
                        businessJson = RelayDataHelper.ModifyStringCustomFields2(businessJson, ManagerCustomField.BusinessReferenceGuid, JsonConvert.SerializeObject(businessReference, Formatting.Indented));

                        // Create combined object for API call
                        var combinedApiObject = new
                        {
                            OpenDayRequest = openDayRequest,
                            OpenDayResponse = openDayResponse.GetContentAs<OpenDayResponse>(),
                            GetStatusResponse = deviceStatus,
                            ApiReference = new
                            {
                                ApiUrl = $"{model.Api}/business-details-form/38cf4712-6e95-4ce1-b53a-bff03edad273",
                                SecretKey = model.Token,
                                Payload = businessJson
                            }
                        };

                        // Serialize the combined object to JSON
                        string combinedJson = JsonConvert.SerializeObject(combinedApiObject, Formatting.Indented);

                        return Ok(combinedJson);
                    }
                    return StatusCode((int)openDayResponse.StatusCode, openDayResponse.GetContentAsString());
                }

                return StatusCode((int)openDayResponse.StatusCode, openDayResponse.GetContentAsString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, Json(new { Error = ex.Message }));
            }
        }

        public async Task<IActionResult> AjaxCloseDay([FromForm] CloseDayViewModel model)
        {
            try
            {
                string jsonBusiness = model.BusinessDetailJson;

                BusinessReference BusinessReference = new BusinessReference();
                FiscalDaySummary FiscalDaySummary = new FiscalDaySummary();
                CertificateInfo CertificateInfo = new CertificateInfo();

                string brf = RelayDataHelper.FindStringValueInJson(jsonBusiness, ManagerCustomField.BusinessReferenceGuid);
                if (!string.IsNullOrEmpty(brf))
                {
                    BusinessReference = JsonConvert.DeserializeObject<BusinessReference>(brf);
                }

                string fds = RelayDataHelper.FindStringValueInJson(jsonBusiness, ManagerCustomField.FiscalDaySummaryGuid);
                if (!string.IsNullOrEmpty(fds))
                {
                    FiscalDaySummary = JsonConvert.DeserializeObject<FiscalDaySummary>(fds);
                }

                var base64CerInfo = RelayDataHelper.FindStringValueInJson(jsonBusiness, ManagerCustomField.CertificateInfoGuid);
                if (!string.IsNullOrEmpty(base64CerInfo))
                {
                    CertificateInfo = ObjectCompressor.DeserializeFromBase64String<CertificateInfo>(base64CerInfo);
                }

                var deviceID = CertificateInfo.DeviceID;
                var deviceModelName = CertificateInfo.DeviceModelName;
                var deviceModelVersion = CertificateInfo.DeviceModelVersion;

                var closeDayRequest = new CloseDayRequest();

                closeDayRequest.FiscalDayNo = BusinessReference.LastFiscalDayNo;

                closeDayRequest.FiscalDayCounters = FiscalDayProcessor.GetFiscalDayCounter(FiscalDaySummary);

                var counterHash = FiscalDayProcessor.ToHashString(closeDayRequest.FiscalDayCounters.ToList());

                string SourcesHash = CertificateInfo.DeviceID.ToString("F0") +
                    closeDayRequest.FiscalDayNo +
                    BusinessReference.FiscalDayOpened.ToString("yyyy-MM-dd") +
                    counterHash;

                Console.WriteLine(SourcesHash);

                byte[] hashByte = RSA_CryptoHelper.ComputeSHA256Hash(SourcesHash.ToUpper());

                RSA privateKey = RSA_CryptoHelper.ConvertPrivateKeyFromBase64(CertificateInfo.PrivateKey);
                byte[] signatureByte = RSA_CryptoHelper.SignDocument(privateKey, hashByte);

                closeDayRequest.FiscalDayDeviceSignature = new SignatureData()
                {
                    Hash = hashByte,
                    Signature = signatureByte ?? Encoding.UTF8.GetBytes("")
                };

                closeDayRequest.ReceiptCounter = BusinessReference.LastReceiptCounter;


                ApiHelper apiHelper = new ApiHelper(CertificateInfo.Base64Pfx, CertificateInfo.IntegrationType);
                ServerResponse closeDayResponse = await apiHelper.SendPostRequestAsync<CloseDayRequest>(
                    "CloseDay",
                    deviceID,
                    deviceModelName,
                    deviceModelVersion,
                    closeDayRequest
                );

                GetStatusResponse deviceStatus;

                if (closeDayResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    ServerResponse statusresponse = await apiHelper.SendGetRequestAsync(
                        "GetStatus",
                        deviceID,
                        deviceModelName,
                        deviceModelVersion
                    );

                    if (statusresponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        deviceStatus = statusresponse.GetContentAs<GetStatusResponse>();

                        if (deviceStatus.FiscalDayStatus != ZimraEGS.ApiClient.Enums.FiscalDayStatus.FiscalDayClosed || deviceStatus.FiscalDayStatus != ZimraEGS.ApiClient.Enums.FiscalDayStatus.FiscalDayCloseInitiated)
                        {
                            return StatusCode((int)System.Net.HttpStatusCode.BadRequest, new { Error = statusresponse.GetFullResponseAsJson() });
                        }

                        // Create combined object
                        var combinedApiObject = new
                        {
                            closeDayRequest,
                            CloseDayResponse = closeDayResponse.GetContentAs<CloseDayResponse>(),
                            GetStatusResponse = deviceStatus,
                        };

                        // Serialize combined object to JSON
                        string combinedJson = JsonConvert.SerializeObject(combinedApiObject, Formatting.Indented);

                        return Ok(combinedJson);
                    }
                }

                return StatusCode((int)closeDayResponse.StatusCode, closeDayResponse.GetContentAsString());

            }
            catch (Exception ex)
            {
                return StatusCode(500, Json(new { Error = ex.Message }));
            }
        }


        [HttpPost]
        public async Task<IActionResult> AjaxIssueCertificate([FromForm] CertInfoViewModel model)
        {
            try
            {
                var deviceID = model.CertificateInfo.DeviceID;
                var deviceModelName = model.CertificateInfo.DeviceModelName;
                var deviceModelVersion = model.CertificateInfo.DeviceModelVersion;

                IssueCertificateRequest IssueCertificateRequest = new IssueCertificateRequest()
                {
                    CertificateRequest = model.CertificateInfo.GeneratedCSR
                };

                ApiHelper apiHelper = new ApiHelper(model.CertificateInfo.Base64Pfx, model.CertificateInfo.IntegrationType);
                ServerResponse issueCertificateResponse = await apiHelper.SendPostRequestAsync<IssueCertificateRequest>(
                    "IssueCertificate",
                    deviceID,
                    deviceModelName,
                    deviceModelVersion,
                    IssueCertificateRequest
                );

                if (issueCertificateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    IssueCertificateResponse issuedCertificate = issueCertificateResponse.GetContentAs<IssueCertificateResponse>();

                    ServerResponse gerConfigResponse = await apiHelper.SendGetRequestAsync(
                        "GetConfig",
                        deviceID,
                        deviceModelName,
                        deviceModelVersion
                    );

                    if (gerConfigResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var deviceConfig = gerConfigResponse.GetContentAs<GetConfigResponse>();

                        CertificateInfo newCertificateInfo = model.CertificateInfo;
                        newCertificateInfo.DeviceCertificate = Utilities.CleanBase64String(issuedCertificate.Certificate);
                        string pfxbyte = Utilities.GeneratePfx(Utilities.CleanBase64String(model.CertificateInfo.PrivateKey), newCertificateInfo.DeviceCertificate, (string)null);
                        newCertificateInfo.Base64Pfx = pfxbyte;
                        newCertificateInfo.CertificateValidTill = deviceConfig.CertificateValidTill;

                        model.CertificateInfo = newCertificateInfo;
                        string Base64Certificate = ObjectCompressor.SerializeToBase64String(newCertificateInfo);

                        // Modify the business JSON with the new values
                        var businessJson = model.BusinessDetailJson;
                        businessJson = RelayDataHelper.ModifyStringCustomFields2(businessJson, ManagerCustomField.CertificateInfoGuid, Base64Certificate);

                        // Create combined object for API call
                        var combinedApiObject = new
                        {
                            CertificateInfo = model.CertificateInfo,
                            IssueCertificateResponse = issueCertificateResponse.GetFullResponseAsJson(),
                            ApiReference = new
                            {
                                ApiUrl = $"{model.Api}/business-details-form/38cf4712-6e95-4ce1-b53a-bff03edad273",
                                SecretKey = model.Token,
                                Payload = businessJson
                            }
                        };

                        // Serialize the combined object to JSON
                        string combinedJson = JsonConvert.SerializeObject(combinedApiObject, Formatting.Indented);

                        return Ok(combinedJson);
                    }
                    return StatusCode((int)issueCertificateResponse.StatusCode, issueCertificateResponse.GetContentAsString());
                }

                return StatusCode((int)issueCertificateResponse.StatusCode, issueCertificateResponse.GetContentAsString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, Json(new { Error = ex.Message }));
            }
        }
    }
}
