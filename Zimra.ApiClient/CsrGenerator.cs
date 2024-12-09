using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Zimra.ApiClient
{
    public class CsrGenerator
    {
        /// <summary>
        /// Generates a CSR (Certificate Signing Request) and private key for a given key type (RSA or EC).
        /// </summary>
        /// <param name="cn">Common Name (CN) for the CSR Subject.</param>
        /// <param name="keyType">Key type, either "RSA" or "EC".</param>
        /// <returns>A tuple containing the CSR in PEM format and the private key in PEM format.</returns>
        public static (string csr, string privateKey) GenerateCsr(string cn, string keyType)
        {
            AsymmetricAlgorithm key;
            CertificateRequest request;

            if (keyType == "RSA")
            {
                using (RSA rsa = RSA.Create(2048)) // Using RSA with a 2048-bit key size
                {
                    request = CreateCertificateRequest(cn, rsa);
                    string privateKeyPem = ExportRsaPrivateKeyToPem(rsa);
                    byte[] csrBytes = request.CreateSigningRequest();
                    string csrPem = ConvertToPem("CERTIFICATE REQUEST", csrBytes);
                    return (csrPem, privateKeyPem);
                }
            }
            else if (keyType == "EC")
            {
                using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256)) // Using EC with NIST P-256 curve
                {
                    request = CreateCertificateRequest(cn, ecdsa);
                    string privateKeyPem = ExportEccPrivateKeyToPem(ecdsa);
                    byte[] csrBytes = request.CreateSigningRequest();
                    string csrPem = ConvertToPem("CERTIFICATE REQUEST", csrBytes);
                    return (csrPem, privateKeyPem);
                }
            }
            else
            {
                throw new ArgumentException("Unsupported key type. Only RSA and EC are supported.", nameof(keyType));
            }
        }

        /// <summary>
        /// Creates a certificate request with a specified key.
        /// </summary>
        /// <param name="cn">Common Name (CN) for the CSR Subject.</param>
        /// <param name="key">Asymmetric key (RSA or ECDsa).</param>
        /// <returns>CertificateRequest object.</returns>
        private static CertificateRequest CreateCertificateRequest(string cn, AsymmetricAlgorithm key)
        {
            // Subject for CSR
            var distinguishedName = new X500DistinguishedName($"CN={cn},C=ZW,O=Zimbabwe Revenue Authority,S=Zimbabwe");

            // Check key type and configure CertificateRequest accordingly
            if (key is RSA rsaKey)
            {
                return new CertificateRequest(distinguishedName, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            else if (key is ECDsa ecdsaKey)
            {
                return new CertificateRequest(distinguishedName, ecdsaKey, HashAlgorithmName.SHA256);
            }
            else
            {
                throw new ArgumentException("Unsupported key type. Only RSA and ECDSA are supported.", nameof(key));
            }
        }

        /// <summary>
        /// Converts raw bytes to a PEM-formatted string.
        /// </summary>
        private static string ConvertToPem(string label, byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"-----BEGIN {label}-----");
            builder.AppendLine(Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine($"-----END {label}-----");
            return builder.ToString();
        }

        /// <summary>
        /// Exports an EC private key to PEM format.
        /// </summary>
        private static string ExportEccPrivateKeyToPem(ECDsa ecdsa)
        {
            byte[] privateKeyBytes = ecdsa.ExportPkcs8PrivateKey(); // ExportECPrivateKey();
            return ConvertToPem("EC PRIVATE KEY", privateKeyBytes);
        }

        /// <summary>
        /// Exports an RSA private key to PEM format.
        /// </summary>
        private static string ExportRsaPrivateKeyToPem(RSA rsa)
        {
            byte[] privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            return ConvertToPem("PRIVATE KEY", privateKeyBytes);
        }
    }
}
