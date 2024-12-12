using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ZimraEGS.ApiClient.Enums;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;
using ZimraEGS.Helpers;
using ZimraEGS.Models;

namespace ZimraEGS.Controllers
{
    public class RelayController : Controller
    {
        [HttpPost("relay")]
        public async Task<IActionResult> ProcessFormData([FromForm] Dictionary<string, string> formData)
        {
            try
            {
                var relayData = new RelayData(formData);

                if (relayData.CertificateInfo == null)
                {
                    SetupViewModel setupViewModel = new SetupViewModel
                    {
                        Referrer = relayData.Referrer,
                        Api = relayData.Api,
                        Token = relayData.Token,
                        BusinessDetailsJson = relayData.BusinessDetailJson
                    };

                    TempData["SetupViewModel"] = JsonConvert.SerializeObject(setupViewModel);

                    return RedirectToAction("Register", "Setup");

                }

                var deviceID = relayData.CertificateInfo.DeviceID;
                var deviceModelName = relayData.CertificateInfo.DeviceModelName;
                var deviceModelVersion = relayData.CertificateInfo.DeviceModelVersion;

                ApiHelper apiHelper = new ApiHelper(relayData.CertificateInfo.Base64Pfx, relayData.CertificateInfo.IntegrationType);

                ServerResponse statusresponse = await apiHelper.SendGetRequestAsync(
                    "GetStatus",
                    deviceID,
                    deviceModelName,
                    deviceModelVersion
                );

                if (statusresponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    relayData.DeviceStatus = statusresponse.GetContentAs<GetStatusResponse>();

                    if (relayData.DeviceStatus.FiscalDayStatus == FiscalDayStatus.FiscalDayClosed)
                    {
                        // Check Certificate Valid
                        TimeSpan dateDifference = relayData.CertificateInfo.CertificateValidTill - DateTimeOffset.Now;
                        if (dateDifference.TotalDays < 30)
                        {
                            CertInfoViewModel certIfoVew = new CertInfoViewModel
                            {
                                BusinessDetailJson = relayData.BusinessDetailJson,
                                Api = relayData.Api,
                                Token = relayData.Token,
                                Referrer = relayData.Referrer,
                                CertificateInfo = relayData.CertificateInfo,
                            };

                            TempData["CertIfoVewModel"] = JsonConvert.SerializeObject(certIfoVew);
                            return RedirectToAction("IssueCertificate", "FiscalDevice");

                        }

                        OpenDayViewModel openDayViewModel = relayData.GetOpenDayViewModel();
                        TempData["OpenDayViewModel"] = JsonConvert.SerializeObject(openDayViewModel);
                        return RedirectToAction("OpenDay", "FiscalDevice");
                    }
                    else
                    {
                        // Check if invoice Has been Approved
                        // Check if invoice has been approved
                        if (relayData.ApprovalStatus == "APPROVED")
                        {
                            ApprovedInvoiceViewModel approvedInvoiceViewModel = relayData.GetApprovedInvoiceViewModel();
                            return View("ApprovedInvoice", approvedInvoiceViewModel);
                        }
                        else if (string.IsNullOrEmpty(relayData.ApprovalStatus) && relayData.BusinessReference.LastApprovalStatus == "REJECTED")
                        {
                            return RedirectToAction("Error", "Home");
                        }
                    }
                }
                else
                {
                    return StatusCode((int)statusresponse.StatusCode, new { Error = statusresponse.GetFullResponseAsJson() });
                }

                ServerResponse configresponse = await apiHelper.SendGetRequestAsync(
                    "GetConfig",
                    deviceID,
                    deviceModelName,
                    deviceModelVersion
                );

                if (Response.StatusCode == 200)
                {
                    relayData.DeviceConfig = configresponse.GetContentAs<GetConfigResponse>();
                }
                else
                {
                    return StatusCode((int)configresponse.StatusCode, new { Error = configresponse.GetFullResponseAsJson() });
                }

                ReceiptHelper rh = new ReceiptHelper(relayData);
                relayData.Receipt = rh.GenerateZimraReceipt();

                RelayDataViewModel viewModel = relayData.GetRelayDataViewModel();

                return View(viewModel);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Internal server error: {ex.Message}" });
            }
        }
        

        public async Task<IActionResult> AjaxSubmitReceipt([FromForm] RelayDataViewModel model)
        {
            try
            {
                var deviceID = model.DeviceID;
                var deviceModelName = model.DeviceModelName;
                var deviceModelVersion = model.DeviceModelVersion;

                string ApprovalStatus = string.Empty;

                var payload = new SubmitReceiptRequest
                {
                    Receipt = JsonConvert.DeserializeObject<Receipt>(model.ReceiptJson)
                };

                ApiHelper apiHelper = new ApiHelper(model.Base64Pfx, model.IntegrationType);

                // Deserialize receipt JSON and make the API call
                ServerResponse response = await apiHelper.SendPostRequestAsync<SubmitReceiptRequest>(
                    "SubmitReceipt",
                    deviceID,
                    deviceModelName,
                    deviceModelVersion,
                    payload
                );

                //GetStatusResponse getStatusResponse;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    SubmitReceiptResponse submitReceiptResponse = response.GetContentAs<SubmitReceiptResponse>();

                    ApprovalStatus = "APPROVED";

                    if (submitReceiptResponse.ValidationErrors != null)
                    {
                        var hasCriticalErrors = submitReceiptResponse.ValidationErrors.Any(error =>
                                    error.ValidationErrorColor.Equals("Grey", StringComparison.OrdinalIgnoreCase) ||
                                    error.ValidationErrorColor.Equals("Red", StringComparison.OrdinalIgnoreCase)
                                );

                        if (hasCriticalErrors)
                        {
                            ApprovalStatus = "REJECTED";
                        }
                    }

                    // Process Invoice JSON
                    var invoiceJson = model.InvoiceJson;

                    invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.ApprovalStatusGuid, ApprovalStatus);
                    invoiceJson = RelayDataHelper.UpdateOrCreateField(invoiceJson, model.ReceiptType == ReceiptType.FiscalInvoice ? "IssueDate" : "Date", model.InvoiceDate);
                    invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.ReceiptDateGuid, model.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss"));

                    invoiceJson = RelayDataHelper.UpdateOrCreateField(invoiceJson, "Reference", model.InvoiceNumber);

                    InvoiceReference invoiceReference = new InvoiceReference();
                    invoiceReference.ApprovalStatus = ApprovalStatus;
                    invoiceReference.ReceiptHash = model.ReceiptHash;
                    invoiceReference.ReceiptSignature = model.ReceiptSignature;
                    invoiceReference.SubmitReceiptResponse = submitReceiptResponse;

                    model.InvoiceReferenceJson = JsonConvert.SerializeObject(invoiceReference, Formatting.Indented);

                    invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.ReceiptReferenceGuid, model.InvoiceReferenceJson);

                    invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.ReceiptQRCodeGuid, model.ReceiptQrCode);

                    if (!string.IsNullOrEmpty(ApprovalStatus))
                    {
                        invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.VerificationCodeGuid, model.TmpReceiptVerificationCode);
                    }

                    invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.DeviceSNGuid, model.DeviceSN);
                    invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.DeviceIDGuid, model.DeviceID);
                    invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.FiscalDayNoGuid, model.FiscalDayNo);
                    invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.ReceiptGlobalNoGuid, model.ReceiptGlobalNo);
                    invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.ReceiptCounterGuid, model.ReceiptCounter);

                    if (model.ReceiptType != ReceiptType.FiscalInvoice)
                    {
                        invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.DeviceIDRefGuid, model.DeviceIDRef);
                        invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.DeviceSNRefGuid, model.DeviceSNRef);
                        invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.FiscalDayNoRefGuid, model.FiscalDayNoRef);
                        invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.ReceiptGlobalNoRefGuid, model.ReceiptGlobalNoRef);
                        invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.ReceiptCounterRefGuid, model.ReceiptCounterRef);

                        invoiceJson = RelayDataHelper.ModifyDecimalCustomFields2(invoiceJson, ManagerCustomField.ReceiptRefNoGuid, model.ReceiptRefNo);
                        invoiceJson = RelayDataHelper.ModifyStringCustomFields2(invoiceJson, ManagerCustomField.ReceiptRefDateGuid, model.ReceiptRefDate);
                    }

                    // Process Business Details JSON

                    var businessJson = model.BusinessDetailJson;

                    BusinessReference businessreference = new BusinessReference();
                    if (!string.IsNullOrEmpty(model.BusinessReferenceJson))
                    {
                        businessreference = JsonConvert.DeserializeObject<BusinessReference>(model.BusinessReferenceJson);
                        if (businessreference == null) { businessreference = new BusinessReference(); }
                    }

                    businessreference.IntegrationType = model.IntegrationType;
                    businessreference.DeviceID = model.DeviceID;
                    businessreference.DeviceSerialNumber = model.DeviceSN;
                    businessreference.FiscalDayStatus = model.FiscalDayStatus;
                    businessreference.LastFiscalDayNo = model.FiscalDayNo;

                    businessreference.LastApprovalStatus = ApprovalStatus;

                    businessreference.LastReceiptCounter = model.ReceiptCounter;

                    if (ApprovalStatus == "APPROVED")
                    {
                        businessreference.LastReceiptGlobalNo = model.ReceiptGlobalNo;
                        businessreference.LastReceiptHash = model.ReceiptHash;

                        businessJson = RelayDataHelper.ModifyStringCustomFields2(businessJson, ManagerCustomField.BusinessReferenceGuid, JsonConvert.SerializeObject(businessreference, Formatting.Indented));

                        if (!string.IsNullOrEmpty(model.ReceiptJson))
                        {
                            var receipt = JsonConvert.DeserializeObject<Receipt>(model.ReceiptJson);

                            var newFiscalDaySummary = FiscalDayProcessor.GenerateFiscalDaySummary(receipt, model.FiscalDayNo);

                            if (!string.IsNullOrEmpty(model.FiscalDaySummaryJson?.Trim()) && model.FiscalDaySummaryJson.Trim() != "null")
                            {
                                var lastFiscalDaySummary = JsonConvert.DeserializeObject<FiscalDaySummary>(model.FiscalDaySummaryJson);
                                newFiscalDaySummary = FiscalDayProcessor.CombineFiscalDaySummaries(lastFiscalDaySummary, newFiscalDaySummary);
                            }

                            string fiscalDaySummaryJson = JsonConvert.SerializeObject(newFiscalDaySummary, Formatting.Indented);

                            // Modify the business JSON with the new fiscal day summary.
                            businessJson = RelayDataHelper.ModifyStringCustomFields2(businessJson, ManagerCustomField.FiscalDaySummaryGuid, fiscalDaySummaryJson);
                        }
                    }

                    // Create combined object
                    var combinedApiObject = new
                    {
                        ApiInvoice = new
                        {
                            ApiUrl = Utils.ConstructInvoiceApiUrl(model.Referrer, model.FormKey),
                            SecretKey = model.Token,
                            Payload = invoiceJson
                        },
                        ApiReference = new
                        {
                            ApiUrl = $"{model.Api}/business-details-form/38cf4712-6e95-4ce1-b53a-bff03edad273",
                            SecretKey = model.Token,
                            Payload = businessJson
                        },
                        SubmitReceiptResponse = submitReceiptResponse,
                        //GetStatusResponse = getStatusResponse
                    };

                    // Serialize combined object to JSON
                    string combinedJson = JsonConvert.SerializeObject(combinedApiObject, Formatting.Indented);

                    return Ok(combinedJson);
                }

                return StatusCode((int)response.StatusCode, response.GetContentAsString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, Json(new { Error = ex.Message }));
            }
        }
    }
}
