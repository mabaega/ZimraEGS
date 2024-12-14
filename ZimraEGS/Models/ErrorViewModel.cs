namespace ZimraEGS.Models
{
    public class ErrorViewModel
    {
        public string Referrer { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
