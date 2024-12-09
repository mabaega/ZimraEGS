namespace Zimra.ApiClient.Enums
{
    using System = System;

    public enum PosUserStatusEnum
    {

        [System.Runtime.Serialization.EnumMember(Value = @"NotConfirmed")]
        NotConfirmed = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"Active")]
        Active = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"Blocked")]
        Blocked = 2,

    }


}