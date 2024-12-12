namespace ZimraEGS.ApiClient.Enums
{
    using System = System;

    public enum MoneyType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"Cash")]
        Cash = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"Card")]
        Card = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"MobileWallet")]
        MobileWallet = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"Coupon")]
        Coupon = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"Credit")]
        Credit = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"BankTransfer")]
        BankTransfer = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"Other")]
        Other = 6,

    }

}
