namespace ZimraEGS.Models
{
    public static class ManagerCustomField
    {
        // businessDetails & Invoice
        public const string BusinessReferenceGuid = "f7214db4-6726-4aa9-b5cd-3ff90ce9ba6c"; //readOnly
        public const string FiscalDaySummaryGuid = "0f0bf167-4b63-493d-ab45-049a76a07f46"; //readOnly
        public const string CertificateInfoGuid = "6a347c55-735a-4a38-8cf6-0db93dce2ded"; //readOnly

        // Customers & Invoice

        public const string BuyerRegisterNameGuid = "d554566a-188b-4c5c-abec-86e1f18eee78";
        public const string BuyerTradeNameGuid = "c8def7d7-8d4d-487c-9685-ab0693dccd1f";
        public const string BuyerTINGuid = "af771746-d00a-4f78-906c-8946098e1184";
        public const string BuyerVatNumberGuid = "2a0e1d10-e66b-4b12-9b9c-3b3f3b7c35f9";
        public const string BuyerEmailGuid = "5b2a8521-9e28-4edf-ac2c-527562349188";
        public const string BuyerPhoneNoGuid = "2e909259-6785-419a-aaef-6390a427b5f6";
        public const string BuyerProvinceGuid = "aec2c1f7-3aed-49fd-82d8-c15a7b69a74f";
        public const string BuyerCityGuid = "5fc8a9b9-efef-45d0-94da-1b0438e75725";
        public const string BuyerStreetGuid = "62e8d3ee-bf4d-4b3e-956f-870ed4d99a90";
        public const string BuyerHouseNoGuid = "568a19c2-bc66-4dc2-8bbd-de0768e7e283";

        // Invoice, Credit Note & debit Note
        public const string PaymentType1Guid = "5b3ebf5f-ad4f-4ffc-84d1-abda67a7d294"; //clear on create
        public const string PaymentAmount1Guid = "e2ab11ef-73e2-4ea4-802f-55b8c834037d"; //clear on create
        public const string PaymentType2Guid = "cb790f2c-f1b2-41d7-9b37-2ba19af4d797"; //clear on create
        public const string PaymentAmount2Guid = "f3a2f5dd-8da8-4701-b8d5-be5877792a79"; //clear on create

        public const string ReceiptDateGuid = "39b3d219-a386-4d9f-b3a0-55968b4bc7b7"; //readOnly + clear on create
        public const string ApprovalStatusGuid = "fc6f05f9-50c7-46c0-b9f5-7fe0ca83cf2a"; //readOnly + clear on create
        public const string ReceiptQRCodeGuid = "7eaadfa6-1fb9-4dce-9a1c-9d74beb1d0f7"; //readOnly + clear on create
        public const string VerificationCodeGuid = "a0454495-cae8-43ef-a418-4dad69427e78"; //readOnly + hide + clear on create
        public const string ServerResponseGuid = "75190339-992a-4fb0-bf79-f5f8226edb4b"; //readOnly + clear on create

        public const string DeviceIDGuid = "d08c8744-486f-470d-82d1-29b1a1cb06ae"; //readOnly + hide + clear on create
        public const string DeviceSNGuid = "a360538c-ebe9-49e9-8cf8-fccd51320380"; //readOnly + hide + clear on create
        public const string FiscalDayNoGuid = "c6e40e7a-974e-4ffc-9c76-1c1ecfd05693"; //readOnly + hide + clear on create
        public const string ReceiptCounterGuid = "a9867045-740b-493f-8b67-43b6da07e41e"; //readOnly + hide + clear on create
        public const string ReceiptGlobalNoGuid = "4712e5df-c5e3-4b6e-9746-8ebb58a81dee"; //readOnly + hide + clear on create

        //Credit Note & Debit Note

        public const string ReceiptNoRefGuid = "c7e511ba-6e9e-48ad-a61c-084e7d0f38ac";//readOnly + hide * + clear on create
        public const string ReceiptNotesGuid = "c7e511ba-6e9e-48ad-a61c-084e7d0f38ad"; //clear on create

        public const string ReceiptRefDateGuid = "39b3d219-a386-4d9f-b3a0-55968b4bc7b8";//readOnly + hide + clear on create
        public const string DeviceIDRefGuid = "d08c8744-486f-470d-82d1-29b1a1cb06af";//readOnly + hide + clear on create
        public const string DeviceSNRefGuid = "a360538c-ebe9-49e9-8cf8-fccd51320381";//readOnly + hide  + clear on create

        public const string FiscalDayNoRefGuid = "c6e40e7a-974e-4ffc-9c76-1c1ecfd05694"; //readOnly + hide + clear on create
        public const string ReceiptCounterRefGuid = "a9867045-740b-493f-8b67-43b6da07e41f"; //readOnly + hide + clear on create
        public const string ReceiptGlobalNoRefGuid = "4712e5df-c5e3-4b6e-9746-8ebb58a81def"; //readOnly + hide + clear on create

        // Item for VAT
        public const string HsCodeGuid = "329de867-9cf1-4dfe-8b06-5084bce788c7"; //0

        public const string AppVersionGuid = "329de867-9cf1-4dfe-8b06-5084bce788c8";

    }
}
