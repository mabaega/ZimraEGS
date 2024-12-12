namespace ZimraEGS.ApiClient.Enums
{
    using System = System;

    public enum ReceiptType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"FiscalInvoice")]
        FiscalInvoice = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"CreditNote")]
        CreditNote = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"DebitNote")]
        DebitNote = 2,

    }

}
