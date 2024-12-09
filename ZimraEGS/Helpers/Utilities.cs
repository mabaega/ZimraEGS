using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Zimra.ApiClient.Models;
using System.Reflection;
using System.Text;

namespace ZimraEGS.Helpers
{
    public static class Utilities
    {
        public static string GetBaseUrl(EnvironmentType environmentType)
        {
            string environment = environmentType switch
            {
                EnvironmentType.Simulation => "https://fdmsapitest.zimra.co.zw",
                EnvironmentType.Production => "https://fdmsapi.zimra.co.zw",
                _ => "https://fdmsapitest.zimra.co.zw"
            };
            return environment;
        }

        public static string ConstructInvoiceApiUrl(string referrer, string invoiceUUID)
        {
            var uri = new Uri(referrer);
            var baseUrl = $"{uri.Scheme}://{uri.Host}";

            if (uri.Port != 80 && uri.Port != 443)
            {
                baseUrl += $":{uri.Port}";
            }

            if (referrer.Contains("purchase-invoice-view"))
            {
                return $"{baseUrl}/api2/purchase-invoice-form/{invoiceUUID}";
            }
            else if (referrer.Contains("sales-invoice-view"))
            {
                return $"{baseUrl}/api2/sales-invoice-form/{invoiceUUID}";
            }
            else if (referrer.Contains("debit-note-view"))
            {
                return $"{baseUrl}/api2/debit-note-form/{invoiceUUID}";
            }
            else if (referrer.Contains("credit-note-view"))
            {
                return $"{baseUrl}/api2/credit-note-form/{invoiceUUID}";
            }

            throw new ArgumentException("Invalid referrer URL");
        }

        public static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Nama lengkap resource: {Namespace}.{FileName}
            string fullResourceName = $"{assembly.GetName().Name}.{resourceName}";

            using (Stream stream = assembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Resource '{fullResourceName}' not found.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static string CleanBase64String(string base64WithHeader)
        {
            string base64string = Regex.Replace(base64WithHeader, @"-----.*?-----|\s+", "");
            return Regex.Replace(base64string, @"\s+", "");
        }
        public static byte[] GenerateHash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            }
        }
        public static string ComputeMD5(byte[] dataByte)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(dataByte);
                return BitConverter.ToString(hash).Replace("-", "").ToUpper().Substring(0, 16);
            }
        }
        public static (string pfxFilePath, string pfxBase64) GeneratePfx(string privateKeyPem, string certificateBase64, string? password)
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

        private static (string pfxFilePath, string pfxBase64) ExportToPfx(X509Certificate2 certWithKey, string? password)
        {
            // Export to PFX
            var pfxBytes = certWithKey.Export(X509ContentType.Pfx, password);
            string pfxFilePath = Path.Combine(Directory.GetCurrentDirectory(), "output.pfx");
            File.WriteAllBytes(pfxFilePath, pfxBytes);

            // Convert PFX to Base64 string
            string pfxBase64 = Convert.ToBase64String(pfxBytes);
            return (pfxFilePath, pfxBase64);
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
