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

        public long EGSVersion { get; private set; }
        
        public double InvoiceTotal { get; private set; } = 0;
        public string ApprovalStatus { get; set; } = string.Empty;
        public int InvReceiptGlobalNo { get; private set; } = 0;

        public DateTimeOffset? ReceiptDate { get; private set; }

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
        public string ReceiptRefNo { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(LocalDateTimeConverter))]
        public DateTimeOffset ReceiptRefDate { get; set; } = DateTimeOffset.MinValue;

        public string ReceiptPrintForm { get; private set; } = string.Empty;
        public string PaymentType1 { get; private set; } = string.Empty;
        public double PaymentAmount1 { get; private set; } = 0;
        public string PaymentType2 { get; private set; } = string.Empty;
        public double PaymentAmount2 { get; private set; } = 0;

        public string ReceiptQrCode { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public long ReceiptID {  get; set; }

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
            string brf = RelayDataHelper.GetStringCustomField2Value(jsonBusiness, ManagerCustomField.BusinessReferenceGuid);
            if (!string.IsNullOrEmpty(brf))
            {
                BusinessReference = JsonConvert.DeserializeObject<BusinessReference>(brf);
            }

            string fds = RelayDataHelper.GetStringCustomField2Value(jsonBusiness, ManagerCustomField.FiscalDaySummaryGuid);
            if (!string.IsNullOrEmpty(fds))
            {
                FiscalDaySummary = JsonConvert.DeserializeObject<FiscalDaySummary>(fds);
            }

            var base64CerInfo = RelayDataHelper.GetStringCustomField2Value(jsonBusiness, ManagerCustomField.CertificateInfoGuid);
            if (!string.IsNullOrEmpty(base64CerInfo))
            {
                CertificateInfo = ObjectCompressor.DeserializeFromBase64String<CertificateInfo>(base64CerInfo);
            }

            EGSVersion = VersionHelper.GetNumberOnly(RelayDataHelper.GetStringCustomField2Value(jsonBusiness, ManagerCustomField.AppVersionGuid));
        }

        private void InitializeInvoiceData(string jsonInvoice)
        {
            ApprovalStatus = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.ApprovalStatusGuid);

            if (!string.IsNullOrEmpty(ApprovalStatus))
            {
                InvReceiptGlobalNo = GetIntValue(jsonInvoice, ManagerCustomField.ReceiptGlobalNoGuid);
                ReceiptDate = GetDateTimeValue(jsonInvoice, ManagerCustomField.ReceiptDateGuid);

                var receiptRef = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.ServerResponseGuid);
                ReceiptReference = JsonConvert.DeserializeObject<InvoiceReference>(receiptRef);

                //ReceiptID = ReceiptReference.SubmitReceiptResponse.ReceiptID;
            }

            PaymentType1 = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.PaymentType1Guid);
            PaymentAmount1 = GetDoubleValue(jsonInvoice, ManagerCustomField.PaymentAmount1Guid);

            PaymentType2 = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.PaymentType2Guid);
            PaymentAmount2 = GetDoubleValue(jsonInvoice, ManagerCustomField.PaymentAmount2Guid);

            BuyerRegisterName = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerRegisterNameGuid, "InvoiceParty");
            BuyerTradeName = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerTradeNameGuid, "InvoiceParty");
            BuyerTIN = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerTINGuid, "InvoiceParty");
            BuyerVatNumber = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerVatNumberGuid, "InvoiceParty");
            BuyerEmail = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerEmailGuid, "InvoiceParty");
            BuyerPhoneNo = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerPhoneNoGuid, "InvoiceParty");
            BuyerProvince = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerProvinceGuid, "InvoiceParty");
            BuyerCity = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerCityGuid, "InvoiceParty");
            BuyerStreet = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerStreetGuid, "InvoiceParty");
            BuyerHouseNo = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.BuyerHouseNoGuid, "InvoiceParty");

            ReceiptQrCode = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.ReceiptQRCodeGuid);
            VerificationCode = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.VerificationCodeGuid);

            ReceiptPrintForm = "InvoiceA4";

            CurrencyCode = RelayDataHelper.FindStringValueInJson(jsonInvoice, "Code", "Currency") ?? "ZWG";
        }


        private void DetermineInvoiceType(string jsonInvoice)
        {
            ReceiptType = ReceiptType.FiscalInvoice;

            if (Referrer.Contains("credit-note-view"))
            {
                ReceiptNotes = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.ReceiptNotesGuid) ?? string.Empty;

                DeviceIDRef = GetIntValue(jsonInvoice, ManagerCustomField.DeviceIDGuid, "RefInvoice");
                DeviceSNRef = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.DeviceSNGuid, "RefInvoice");
                FiscalDayNoRef = GetIntValue(jsonInvoice, ManagerCustomField.FiscalDayNoGuid, "RefInvoice");
                ReceiptCounterRef = GetIntValue(jsonInvoice, ManagerCustomField.ReceiptCounterGuid, "RefInvoice");
                ReceiptGlobalNoRef = GetIntValue(jsonInvoice, ManagerCustomField.ReceiptGlobalNoGuid, "RefInvoice");

                ReceiptRefNo = RelayDataHelper.FindStringValueInJson(jsonInvoice, "Reference", "RefInvoice");
                ReceiptRefDate = GetDateTimeValue(jsonInvoice, ManagerCustomField.ReceiptDateGuid, "RefInvoice") ?? DateTimeOffset.MinValue;

                var receiptRefRef = RelayDataHelper.GetStringCustomField2Value(jsonInvoice, ManagerCustomField.ServerResponseGuid, "RefInvoice");
                InvoiceReference ReceiptReferenceRef = JsonConvert.DeserializeObject<InvoiceReference>(receiptRefRef);
                ReceiptID = ReceiptReferenceRef.SubmitReceiptResponse.ReceiptID;

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
            return (int)RelayDataHelper.GetDecimalCustomField2Value(json, key, rootKey);
        }
        private double GetDoubleValue(string json, string key, string rootKey = null)
        {
            return (double)RelayDataHelper.GetDecimalCustomField2Value(json, key, rootKey);
        }
        private DateTimeOffset? GetDateTimeValue(string json, string key, string rootKey = null)
        {
            return DateTimeOffset.TryParse(RelayDataHelper.GetStringCustomField2Value(json, key, rootKey), out DateTimeOffset value) ? value : null;
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
            DateTimeOffset fiscalDayEnd = BusinessReference.FiscalDayOpened.AddHours(CertificateInfo.TaxPayerDayMaxHrs);
            TimeSpan timeDifference = DateTimeOffset.Now - fiscalDayEnd;
            bool TimeForCloseDay = timeDifference.TotalHours < CertificateInfo.TaxpayerDayEndNotificationHrs;

            var viewModel = new ApprovedInvoiceViewModel
            {
                ReceiptReferenceJson = JsonConvert.SerializeObject(ReceiptReference, Formatting.Indented),
                Referrer = Referrer,
                ReceiptVerificationCode = VerificationCode,
                ReceiptQrCode = ReceiptQrCode,
                Api = Api,
                Token= Token,
                BusinessDetailJson= BusinessDetailJson,
                FiscalDayStatus= DeviceStatus.FiscalDayStatus,
                TimeForCloseDay = TimeForCloseDay
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
            viewModel.ReceiptDate = Receipt.ReceiptDate;

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

            viewModel.ReceiptErrors = ReceiptValidator.ValidateReceipt(Receipt, CertificateInfo.ApplicableTaxes);

            return viewModel;

        }
    }
}