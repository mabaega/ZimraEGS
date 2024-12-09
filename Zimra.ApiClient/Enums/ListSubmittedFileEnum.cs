namespace Zimra.ApiClient.Enums
{
    using System = System;

    public enum ListSubmittedFileEnum
    {

        [System.Runtime.Serialization.EnumMember(Value = @"DeviceId")]
        DeviceId = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"FileName")]
        FileName = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"FileUploadDate")]
        FileUploadDate = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"FileProcessingDate")]
        FileProcessingDate = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"FileProcessingStatus")]
        FileProcessingStatus = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"FileProcessingError")]
        FileProcessingError = 5,

    }

}
