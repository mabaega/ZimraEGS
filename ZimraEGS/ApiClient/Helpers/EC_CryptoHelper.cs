using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ZimraEGS.ApiClient.Helpers
{
    public class EC_CryptoHelper
    {
        public static (string csrPem, string privateKey) GenerateCSR(string commonName)
        {
            // Buat kunci EC menggunakan kurva SECG secp256r1 (NIST P-256)
            ECDsa ecdsaKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);

            // Buat Distinguished Name (DN)
            var dn = new X500DistinguishedName(
                $"CN={commonName}, C=ZW, O=Zimbabwe Revenue Authority, ST=Zimbabwe"
            );

            // Buat permintaan sertifikat (CSR)
            var request = new CertificateRequest(
                dn,
                ecdsaKey,
                HashAlgorithmName.SHA256
            );

            // Konversi CSR ke PEM
            byte[] csrBytes = request.CreateSigningRequest();
            string csrPem = ConvertToPemFormat(csrBytes, "CERTIFICATE REQUEST");

            // Konversi kunci privat ke Base64 untuk penyimpanan
            byte[] privateKeyBytes = ecdsaKey.ExportPkcs8PrivateKey();
            string privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);

            return (csrPem, privateKeyBase64);
        }
        private static string ConvertToPemFormat(byte[] data, string type)
        {
            string base64 = Convert.ToBase64String(data);
            return $"-----BEGIN {type}-----\n{base64}\n-----END {type}-----";
        }
        public static ECDsa ConvertPrivateKeyFromBase64(string privateKeyBase64)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(privateKeyBase64))
                    throw new ArgumentException("Private key cannot be empty");

                byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64);

                ECDsa ecdsaKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                ecdsaKey.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                return ecdsaKey;
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

        public static byte[] SignDocument(ECDsa privateKey, byte[] hash)
        {
            // Tanda tangani hash dengan kunci privat
            return privateKey.SignHash(hash);
        }

        public static bool VerifySignature(X509Certificate2 deviceCertificate, byte[] hash, byte[] signature)
        {
            using (ECDsa publicKey = deviceCertificate.GetECDsaPublicKey())
            {
                if (publicKey == null)
                    throw new InvalidOperationException("Could not extract public key from certificate");

                // Verifikasi tanda tangan
                return publicKey.VerifyHash(hash, signature);
            }
        }

    }
}
