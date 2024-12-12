namespace ZimraEGS.ApiClient.Enums
{
    using System = System;

    public enum FiscalDayProcessingError
    {

        [System.Runtime.Serialization.EnumMember(Value = @"BadCertificateSignature")]
        BadCertificateSignature = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"MissingReceipts")]
        MissingReceipts = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"ReceiptsWithValidationErrors")]
        ReceiptsWithValidationErrors = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"CountersMismatch")]
        CountersMismatch = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"InternalError")]
        InternalError = 4,

    }

}
