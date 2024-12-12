namespace ZimraEGS.ApiClient.Enums
{
    using System = System;

    public enum FiscalDayStatus
    {

        [System.Runtime.Serialization.EnumMember(Value = @"FiscalDayClosed")]
        FiscalDayClosed = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"FiscalDayOpened")]
        FiscalDayOpened = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"FiscalDayCloseInitiated")]
        FiscalDayCloseInitiated = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"FiscalDayCloseFailed")]
        FiscalDayCloseFailed = 3,

    }

}
