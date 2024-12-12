using Newtonsoft.Json;
using ZimraEGS.ApiClient.Enums;
using ZimraEGS.ApiClient.Helpers;
using ZimraEGS.ApiClient.Models;
using ZimraEGS.Helpers;

namespace ZimraEGS.Models
{
    public class RelayData
    {
        public string Referrer { get; private set; } = string.Empty;
        public string FormKey { get; private set; } = string.Empty;
        public string Data { get; private set; } = string.Empty;
        public string Api { get; private set; } = string.Empty;
        public string Token { get; private set; } = string.Empty;
        public double InvoiceTotal { get; private set; } = 0;

        public string ApprovalStatus { get; set; } = string.Empty;
        public int InvReceiptGlobalNo { get; private set; } = 0;

        public DateTimeOffset? InvReceiptDate { get; private set; }

        public string BusinessDetailJson { get; set; } = string.Empty;
        public string InvoiceJson { get; set; } = string.Empty;

        public BusinessReference BusinessReference { get; set; } = new BusinessReference();
        public FiscalDaySummary FiscalDaySummary { get; set; }
        public CertificateInfo CertificateInfo { get; set; }

        public ReceiptType ReceiptType { get; private set; } = ReceiptType.FiscalInvoice;
        public string CurrencyCode { get; private set; } = string.Empty;
        public string ReceiptNotes { get; private set; } = string.Empty;
        public int DeviceIDRef { get; private set; }
        public string DeviceSNRef { get; private set; } = string.Empty;
        public int FiscalDayNoRef { get; private set; }
        public int ReceiptCounterRef { get; private set; }
        public int ReceiptGlobalNoRef { get; private set; }
        public int ReceiptRefNo { get; set; } = 0;
        public string ReceiptRefDate { get; set; } = string.Empty;

        public string ReceiptPrintForm { get; private set; } = string.Empty;
        public string PaymentType1 { get; private set; } = string.Empty;
        public double PaymentAmount1 { get; private set; } = 0;
        public string PaymentType2 { get; private set; } = string.Empty;
        public double PaymentAmount2 { get; private set; } = 0;

        public string ReceiptQrCode { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;

        public string BuyerRegisterName { get; private set; } = string.Empty;
        public string BuyerTradeName { get; private set; } = string.Empty;
        public string BuyerTIN { get; private set; } = string.Empty;
        public string BuyerVatNumber { get; private set; } = string.Empty;
        public string BuyerEmail { get; private set; } = string.Empty;
        public string BuyerPhoneNo { get; private set; } = string.Empty;
        public string BuyerProvince { get; private set; } = string.Empty;
        public string BuyerCity { get; private set; } = string.Empty;
        public string BuyerStreet { get; private set; } = string.Empty;
        public string BuyerHouseNo { get; private set; } = string.Empty;
        public ManagerInvoice ManagerInvoice { get; private set; }
        public GetStatusResponse DeviceStatus { get; set; }
        public GetConfigResponse DeviceConfig { get; set; }

        public Receipt Receipt { get; set; }
        public InvoiceReference ReceiptReference { get; set; }

        public RelayData() { }
        public RelayData(Dictionary<string, string> formData)
        {
            Referrer = GetValue(formData, "Referrer");
            FormKey = GetValue(formData, "Key");
            Data = GetValue(formData, "Data");
            Api = GetValue(formData, "Api");
            Token = GetValue(formData, "Token");

            string invoiceView = GetValue(formData, "View");
            InvoiceTotal = RelayDataHelper.ParseTotalValue(invoiceView);

            BusinessDetailJson = RelayDataHelper.GetValueJson(Data, "BusinessDetails");
            InitializeBusinessData(BusinessDetailJson);

            InvoiceJson = RelayDataHelper.FindStringValueInJson(Data, FormKey);

            var mergedJson = RelayDataHelper.GetJsonDataByGuid(Data, FormKey);
            ManagerInvoice = JsonConvert.DeserializeObject<ManagerInvoice>(mergedJson);

            InitializeInvoiceData(mergedJson);

            DetermineInvoiceType(mergedJson);
        }
        private void InitializeBusinessData(string jsonBusiness)
        {
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
        }

        private void InitializeInvoiceData(string jsonInvoice)
        {
            ApprovalStatus = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.ApprovalStatusGuid);

            if (!string.IsNullOrEmpty(ApprovalStatus))
            {
                InvReceiptGlobalNo = GetIntValue(jsonInvoice, ManagerCustomField.ReceiptGlobalNoGuid);
                //InvReceiptDate = GetDateTimeValue(jsonInvoice, "IssueDate") ?? GetDateTimeValue(jsonInvoice, "Date");
                InvReceiptDate = GetDateTimeValue(jsonInvoice, ManagerCustomField.ReceiptDateGuid) ??
                    GetDateTimeValue(jsonInvoice, "IssueDate") ??
                    GetDateTimeValue(jsonInvoice, "Date");

                var receiptRef = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.ReceiptReferenceGuid);
                ReceiptReference = JsonConvert.DeserializeObject<InvoiceReference>(receiptRef);
            }

            PaymentType1 = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.PaymentType1Guid);

            PaymentType1 = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.PaymentType1Guid);
            PaymentAmount1 = GetDoubleValue(jsonInvoice, ManagerCustomField.PaymentAmount1Guid);
            PaymentType2 = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.PaymentType2Guid);
            PaymentAmount2 = GetDoubleValue(jsonInvoice, ManagerCustomField.PaymentAmount2Guid);

            BuyerRegisterName = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerRegisterNameGuid);
            BuyerTradeName = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerTradeNameGuid);
            BuyerTIN = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerTINGuid);
            BuyerVatNumber = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerVatNumberGuid);
            BuyerEmail = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerEmailGuid);
            BuyerPhoneNo = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerPhoneNoGuid);
            BuyerProvince = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerProvinceGuid);
            BuyerCity = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerCityGuid);
            BuyerStreet = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerStreetGuid);
            BuyerHouseNo = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.BuyerHouseNoGuid);

            ReceiptQrCode = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.ReceiptQRCodeGuid);
            VerificationCode = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.VerificationCodeGuid);

            ReceiptPrintForm = "InvoiceA4";

            CurrencyCode = RelayDataHelper.FindStringValueInJson(jsonInvoice, "Code", "Currency") ?? "ZWG";
        }


        private void DetermineInvoiceType(string jsonInvoice)
        {
            ReceiptType = ReceiptType.FiscalInvoice;

            if (Referrer.Contains("credit-note-view"))
            {
                ReceiptNotes = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.ReceiptNotesGuid) ?? string.Empty;

                DeviceIDRef = GetIntValue(jsonInvoice, ManagerCustomField.DeviceIDGuid, "RefInvoice");
                DeviceSNRef = RelayDataHelper.FindStringValueInJson(jsonInvoice, ManagerCustomField.DeviceSNGuid, "RefInvoice");
                FiscalDayNoRef = GetIntValue(jsonInvoice, ManagerCustomField.FiscalDayNoGuid, "RefInvoice");
                ReceiptCounterRef = GetIntValue(jsonInvoice, ManagerCustomField.ReceiptCounterGuid, "RefInvoice");
                ReceiptGlobalNoRef = GetIntValue(jsonInvoice, ManagerCustomField.ReceiptGlobalNoGuid, "RefInvoice");

                ReceiptRefNo = GetIntValue(jsonInvoice, "Reference", "RefInvoice");
                ReceiptRefDate = RelayDataHelper.FindStringValueInJson(jsonInvoice, "IssueDate", "RefInvoice");

                ReceiptType = ReceiptType.CreditNote;

                var salesUnitPrice = RelayDataHelper.FindStringValueInJson(jsonInvoice, "UnitPrice");
                if (decimal.TryParse(salesUnitPrice, out var salesUnitPriceDecimal) && salesUnitPriceDecimal < 0)
                {
                    ReceiptType = ReceiptType.DebitNote;
                }
            }
        }

        private string GetValue(Dictionary<string, string> formData, string key)
        {
            return formData.GetValueOrDefault(key) ?? string.Empty;
        }
        private int GetIntValue(string json, string key, string rootKey = null)
        {
            return int.TryParse(RelayDataHelper.FindStringValueInJson(json, key, rootKey), out int value) ? value : 0;
        }
        private double GetDoubleValue(string json, string key, string rootKey = null)
        {
            return double.TryParse(RelayDataHelper.FindStringValueInJson(json, key, rootKey), out double value) ? value : 0;
        }
        private DateTimeOffset? GetDateTimeValue(string json, string key, string rootKey = null)
        {
            return DateTimeOffset.TryParse(RelayDataHelper.FindStringValueInJson(json, key, rootKey), out DateTimeOffset value) ? value : null;
        }

        public OpenDayViewModel GetOpenDayViewModel()
        {
            BusinessReference businessReference = BusinessReference ?? new BusinessReference();

            var viewModel = new OpenDayViewModel
            {
                DeviceID = CertificateInfo.DeviceID,
                DeviceModelName = CertificateInfo.DeviceModelName,
                DeviceModelVersion = CertificateInfo.DeviceModelVersion,
                DeviceSerialNumber = CertificateInfo.DeviceSerialNumber,
                LastFiscalDayNo = DeviceStatus.LastFiscalDayNo ?? 0,
                Base64Pfx = CertificateInfo.Base64Pfx,
                IntegrationType = CertificateInfo.IntegrationType,
                BusinessDetailJson = BusinessDetailJson,
                LastReceiptCounter = businessReference.LastReceiptCounter,
                LastReceiptHash = businessReference.LastReceiptHash,
                FiscalDayStatus = DeviceStatus.FiscalDayStatus,
                LastReceiptGlobalNo = DeviceStatus.LastReceiptGlobalNo,
                DeviceStatusJson = JsonConvert.SerializeObject(DeviceStatus, Formatting.Indented),
                Api = Api,
                Token = Token,
                Referrer = Referrer
            };

            return viewModel;
        }

        public CloseDayViewModel GetCloseDayViewModel()
        {
            var viewModel = new CloseDayViewModel
            {
                BusinessDetailJson = BusinessDetailJson,
                Api = Api,
                Token = Token,
                Referrer = Referrer,
            };

            return viewModel;
        }

        public ApprovedInvoiceViewModel GetApprovedInvoiceViewModel()
        {
            var viewModel = new ApprovedInvoiceViewModel
            {
                ReceiptReferenceJson = JsonConvert.SerializeObject(ReceiptReference, Formatting.Indented),
                Referrer = Referrer,
                ReceiptVerificationCode = VerificationCode,
                ReceiptQrCode = ReceiptQrCode,
            };

            return viewModel;
        }

        public RelayDataViewModel GetRelayDataViewModel()
        {
            RelayDataViewModel viewModel = new RelayDataViewModel();

            viewModel.Referrer = Referrer;
            viewModel.FormKey = FormKey;
            viewModel.Api = Api;
            viewModel.Token = Token;
            viewModel.BusinessDetailJson = BusinessDetailJson;
            viewModel.InvoiceJson = InvoiceJson;

            viewModel.DeviceID = CertificateInfo.DeviceID;
            viewModel.DeviceSN = CertificateInfo.DeviceSerialNumber;
            viewModel.DeviceModelName = CertificateInfo.DeviceModelName;
            viewModel.DeviceModelVersion = CertificateInfo.DeviceModelVersion;

            viewModel.Base64Pfx = CertificateInfo.Base64Pfx;
            viewModel.PrivateKey = CertificateInfo.PrivateKey;
            viewModel.IntegrationType = CertificateInfo.IntegrationType;


            viewModel.ReceiptType = Receipt.ReceiptType;
            viewModel.CurrencyCode = Receipt.ReceiptCurrency;

            viewModel.FiscalDayStatus = DeviceStatus.FiscalDayStatus;
            viewModel.FiscalDayNo = DeviceStatus.LastFiscalDayNo ?? 1;

            viewModel.ReceiptGlobalNo = Receipt.ReceiptGlobalNo;
            viewModel.ReceiptCounter = Receipt.ReceiptCounter;

            viewModel.InvoiceNumber = Receipt.InvoiceNo;
            viewModel.InvoiceDate = Receipt.ReceiptDate;

            viewModel.BuyerRegisterName = Receipt.BuyerData.BuyerRegisterName;

            viewModel.ReceiptNotes = ReceiptNotes;
            viewModel.DeviceIDRef = DeviceIDRef;
            viewModel.DeviceSNRef = DeviceSNRef;
            viewModel.FiscalDayNoRef = FiscalDayNoRef;
            viewModel.ReceiptCounterRef = ReceiptCounterRef;
            viewModel.ReceiptGlobalNoRef = ReceiptGlobalNoRef;
            viewModel.ReceiptRefNo = ReceiptRefNo;
            viewModel.ReceiptRefDate = ReceiptRefDate;

            viewModel.TaxAmount = Receipt.ReceiptTaxes.Sum(x => x.TaxAmount);
            viewModel.AmountWithTax = Receipt.ReceiptTaxes.Sum(x => x.SalesAmountWithTax);

            viewModel.InvoiceTotal = Receipt.ReceiptTotal;
            viewModel.InvoiceRoundOff = (Receipt.ReceiptTotal - InvoiceTotal);

            viewModel.ReceiptHash = Convert.ToBase64String(Receipt.ReceiptDeviceSignature.Hash);
            viewModel.ReceiptSignature = Convert.ToBase64String(Receipt.ReceiptDeviceSignature.Signature);

            string md5Signature = Utilities.ComputeMD5(Receipt.ReceiptDeviceSignature.Signature);

            viewModel.ReceiptQrCode = CertificateInfo.QrUrl + '/' +
                CertificateInfo.DeviceID.ToString().PadLeft(10, '0') +
                Receipt.ReceiptDate.ToString("ddMMyyyy") +
                Receipt.ReceiptGlobalNo.ToString().PadLeft(10, '0') +
                md5Signature;

            viewModel.TmpReceiptVerificationCode = string.Format("{0}-{1}-{2}-{3}",
                md5Signature.Substring(0, 4),
                md5Signature.Substring(4, 4),
                md5Signature.Substring(8, 4),
                md5Signature.Substring(12, 4));

            viewModel.ReceiptJson = JsonConvert.SerializeObject(Receipt);

            viewModel.BusinessReferenceJson = JsonConvert.SerializeObject(BusinessReference);
            viewModel.FiscalDaySummaryJson = JsonConvert.SerializeObject(FiscalDaySummary);
            viewModel.InvoiceReferenceJson = JsonConvert.SerializeObject(ReceiptReference);

            DateTimeOffset fiscalDayEnd = BusinessReference.FiscalDayOpened.AddHours(CertificateInfo.TaxPayerDayMaxHrs);
            TimeSpan timeDifference = DateTimeOffset.Now - fiscalDayEnd;
            viewModel.TimeForCloseDay = timeDifference.TotalHours < CertificateInfo.TaxpayerDayEndNotificationHrs;

            return viewModel;

        }
    }
}