namespace Zimra.ApiClient.Enums
{
    using System = System;

    public enum GetUsersListEnum
    {

        [System.Runtime.Serialization.EnumMember(Value = @"UserName")]
        UserName = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"PersonName")]
        PersonName = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"PersonSurname")]
        PersonSurname = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"UserRole")]
        UserRole = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"Email")]
        Email = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"PhoneNo")]
        PhoneNo = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"UserStatus")]
        UserStatus = 6,

    }


}