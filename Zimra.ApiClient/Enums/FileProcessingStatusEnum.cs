namespace Zimra.ApiClient.Enums
{
    using System = System;

    public enum FileProcessingStatusEnum
    {

        [System.Runtime.Serialization.EnumMember(Value = @"InProgress")]
        InProgress = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"CompleteSuccessful")]
        CompleteSuccessful = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"FailedWithErrors")]
        FailedWithErrors = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"WaitingForPreviousFile")]
        WaitingForPreviousFile = 3,

    }

}
