namespace Zimra.ApiClient.Enums
{
    using System = System;

    public enum ReceiptLineType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"Sale")]
        Sale = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"Discount")]
        Discount = 1,

    }

}
