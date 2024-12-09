namespace ZimraEGS.Models
{
    public class SetupViewModel
    {
        public string Referrer { get; set; } = "http://127.0.0.1:55667/summary-view?ogYKWmltcmEgRkRNUw";
        public string BusinessDetails { get; set; } = string.Empty;
        public string BusinessDetailsJson { get; set; } = string.Empty;
        public string Api { get; set; } = "http://127.0.0.1:55667/api2";
        public string Token { get; set; } //= //"CglaaW1yYSBFR1MSEgntuMJCRR4BSBGsdsR1Ojdc8xoSCR/R9DJz4yZNEbxpXoES7TwK"; //"CgpaaW1yYSBGRE1TEhIJrE/XlRdLjEIRnxjrOfuMD50aEgmJ9sQoPzp+SxG+J5YpIB35SA==";
        public bool IsFileReady { get; set; } = false;
        public string FileContent { get; set; } = string.Empty; // Store as Base64 string instead of IFormFile
        public string Filename { get; set; } = string.Empty;
        public CertificateInfo CertificateInfo { get; set; } = new CertificateInfo();
    }
}
