using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ZimraEGS.ApiClient.Helpers
{
    public class RSA_CryptoHelper
    {
        public static (string csrPem, string privateKey) GenerateCSR(string commonName)
        {
            // Generate RSA key
            using (RSA rsaKey = RSA.Create(2048))
            {
                // Create Distinguished Name (DN)
                var dn = new X500DistinguishedName(
                    $"CN={commonName}, C=ZW, O=Zimbabwe Revenue Authority, ST=Zimbabwe"
                );

                // Create certificate request (CSR)
                var request = new CertificateRequest(
                    dn,
                    rsaKey,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );

                // Convert CSR to PEM
                byte[] csrBytes = request.CreateSigningRequest();
                string csrPem = ConvertToPemFormat(csrBytes, "CERTIFICATE REQUEST");

                // Convert private key to Base64 for storage
                byte[] privateKeyBytes = rsaKey.ExportPkcs8PrivateKey();
                string privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

                return (csrPem, privateKeyBase64);
            }
        }

        private static string ConvertToPemFormat(byte[] data, string type)
        {
            string base64 = Convert.ToBase64String(data);
            StringBuilder pem = new StringBuilder();
            pem.AppendLine($"-----BEGIN {type}-----");
            for (int i = 0; i < base64.Length; i += 64)
            {
                pem.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
            }
            pem.AppendLine($"-----END {type}-----");
            return pem.ToString();
        }

        public static RSA ConvertPrivateKeyFromBase64(string privateKeyBase64)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(privateKeyBase64))
                    throw new ArgumentException("Private key cannot be empty");

                byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64);

                RSA rsaKey = RSA.Create();
                rsaKey.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                return rsaKey;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting private key: {ex.Message}");
                throw;
            }
        }

        public static X509Certificate2 ConvertCertificateFromBase64(string certificateBase64)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(certificateBase64))
                    throw new ArgumentException("Certificate cannot be empty");

                byte[] certificateBytes = Convert.FromBase64String(certificateBase64);
                return new X509Certificate2(certificateBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting certificate: {ex.Message}");
                throw;
            }
        }

        public static byte[] ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        public static byte[] SignDocument(RSA privateKey, byte[] hash)
        {
            // Sign the hash with the private key using RSASSA-PKCS1-v1_5 padding
            return privateKey.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public static bool VerifySignature(X509Certificate2 deviceCertificate, byte[] hash, byte[] signature)
        {
            using (RSA publicKey = deviceCertificate.GetRSAPublicKey())
            {
                if (publicKey == null)
                    throw new InvalidOperationException("Could not extract public key from certificate");

                // Verify the signature using RSASSA-PKCS1-v1_5 padding
                return publicKey.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
