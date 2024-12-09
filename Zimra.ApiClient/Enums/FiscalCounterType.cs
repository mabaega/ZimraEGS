namespace Zimra.ApiClient.Enums
{
    using System = System;

    public enum FiscalCounterType
    {

        [System.Runtime.Serialization.EnumMember(Value = @"SaleByTax")]
        SaleByTax = 0,

        [System.Runtime.Serialization.EnumMember(Value = @"SaleTaxByTax")]
        SaleTaxByTax = 1,

        [System.Runtime.Serialization.EnumMember(Value = @"CreditNoteByTax")]
        CreditNoteByTax = 2,

        [System.Runtime.Serialization.EnumMember(Value = @"CreditNoteTaxByTax")]
        CreditNoteTaxByTax = 3,

        [System.Runtime.Serialization.EnumMember(Value = @"DebitNoteByTax")]
        DebitNoteByTax = 4,

        [System.Runtime.Serialization.EnumMember(Value = @"DebitNoteTaxByTax")]
        DebitNoteTaxByTax = 5,

        [System.Runtime.Serialization.EnumMember(Value = @"BalanceByMoneyType")]
        BalanceByMoneyType = 6,

        [System.Runtime.Serialization.EnumMember(Value = @"PayoutByTax")]
        PayoutByTax = 7,

        [System.Runtime.Serialization.EnumMember(Value = @"PayoutTaxByTax")]
        PayoutTaxByTax = 8,

    }

}
