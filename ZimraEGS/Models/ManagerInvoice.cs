namespace ZimraEGS.Models
{
    public class ManagerInvoice
    {
        public DateTimeOffset IssueDate { get; set; }
        public DateTimeOffset DueDateDate { get; set; }
        public string Reference { get; set; }
        public RefInvoice RefInvoice { get; set; }
        public double ExchangeRate { get; set; } = 1;
        public string Description { get; set; }
        public List<Line> Lines { get; set; }
        public bool HasLineNumber { get; set; } = false;
        public bool HasLineDescription { get; set; } = false;
        public bool Discount { get; set; } = false;
        public bool AmountsIncludeTax { get; set; } = false;
    }
    public class Line
    {
        public LineItem Item { get; set; }
        public string LineDescription { get; set; }
        public double Qty { get; set; } = 0;
        public double UnitPrice { get; set; } = 0;
        public double DiscountAmount { get; set; } = 0;
        public TaxCode TaxCode { get; set; }
        public CustomFields2 CustomFields2 { get; set; }
    }
    public class LineItem
    {
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public bool HasDefaultLineDescription { get; set; } = false;
        public string DefaultLineDescription { get; set; }

        public CustomFields2 CustomFields2 { get; set; }
    }

    public class CustomFields2
    {
        public Dictionary<string, string> Strings { get; set; } = [];
        public Dictionary<string, decimal> Decimals { get; set; } = [];
        public Dictionary<string, DateTime?> Dates { get; set; } = [];
        public Dictionary<string, bool> Booleans { get; set; } = [];
        public Dictionary<string, List<string>> StringArrays { get; set; } = [];
    }

    public class TaxCode
    {
        public string Name { get; set; } = "A";
        public double Rate { get; set; } = 0;
    }
    public class RefInvoice
    {
        public string Reference { get; set; }
        public DateTime IssueDate { get; set; }
    }
}