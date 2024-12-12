using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using ZimraEGS.ApiClient.Models;

namespace ZimraEGS.ApiClient.Helpers
{
    public static class Utilities
    {
        public static string GetBaseUrl(PlatformType environmentType)
        {
            string environment = environmentType switch
            {
                PlatformType.Simulation => "https://fdmsapitest.zimra.co.zw",
                PlatformType.Production => "https://fdmsapi.zimra.co.zw",
                _ => "https://fdmsapitest.zimra.co.zw"
            };
            return environment;
        }
        public static string CleanBase64String(string base64WithHeader)
        {
            string base64string = Regex.Replace(base64WithHeader, @"-----.*?-----|\s+", "");
            return Regex.Replace(base64string, @"\s+", "");
        }

        public static string ComputeMD5(byte[] dataByte)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(dataByte);
                return BitConverter.ToString(hash).Replace("-", "").ToUpper().Substring(0, 16);
            }
        }
        public static string GeneratePfx(string privateKeyPem, string certificateBase64, string? password)
        {
            // Convert Base64 string to byte array for the certificate
            byte[] certBytes = Convert.FromBase64String(certificateBase64);

            // Load the certificate
            var certificate = new X509Certificate2(certBytes);

            // Convert PEM private key to DER byte array
            byte[] privateKeyBytes = ConvertPemToDer(privateKeyPem);

            // Attempt to load the private key as PKCS#8
            try
            {
                using (var rsa = RSA.Create())
                {
                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                    var certWithKey = certificate.CopyWithPrivateKey(rsa);
                    return ExportToPfx(certWithKey, password);
                }
            }
            catch (CryptographicException)
            {
                // If RSA import fails, attempt to import as ECDsa
                try
                {
                    using (var ec = ECDsa.Create())
                    {
                        ec.ImportPkcs8PrivateKey(privateKeyBytes, out _);
                        var certWithKey = certificate.CopyWithPrivateKey(ec);
                        return ExportToPfx(certWithKey, password);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to import private key.", ex);
                }
            }
        }

        private static string ExportToPfx(X509Certificate2 certWithKey, string? password)
        {
            // Export to PFX
            var pfxBytes = certWithKey.Export(X509ContentType.Pfx, password);

            // Convert PFX to Base64 string
            string pfxBase64 = Convert.ToBase64String(pfxBytes);

            return pfxBase64;
        }

        // Helper method to convert PEM to DER format
        private static byte[] ConvertPemToDer(string pem)
        {
            // Remove the header and footer
            var base64 = pem.Replace("-----BEGIN PRIVATE KEY-----", "")
                             .Replace("-----END PRIVATE KEY-----", "")
                             .Replace("-----BEGIN EC PRIVATE KEY-----", "")
                             .Replace("-----END EC PRIVATE KEY-----", "")
                             .Replace("\n", "")
                             .Replace("\r", "");

            return Convert.FromBase64String(base64);
        }
    }
}
