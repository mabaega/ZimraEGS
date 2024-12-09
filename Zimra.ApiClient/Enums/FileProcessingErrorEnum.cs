namespace Zimra.ApiClient.Enums
{
    using System = System;

    public enum FileProcessingErrorEnum
    {

        [System.Runtime.Serialization.EnumMember(Value = @"IncorrectFileFormat")]
        IncorrectFileFormat = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"FileSentForClosedDay")]
        FileSentForClosedDay = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"BadCertificateSignature")]
        BadCertificateSignature = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"MissingReceipts")]
        MissingReceipts = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"ReceiptsWithValidationErrors")]
        ReceiptsWithValidationErrors = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"CountersMismatch")]
        CountersMismatch = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"FileExceededAllowedWaitingTime")]
        FileExceededAllowedWaitingTime = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"InternalError")]
        InternalError = 7,

    }

}
