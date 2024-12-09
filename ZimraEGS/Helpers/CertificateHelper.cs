using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;

namespace ZimraEGS.Helpers
{
    public static class CertificateHelper
    {
        public static byte[] GenerateSignature(string hashBase64, string privateKeyPem)
        {
            try
            {
                // Ensure proper PEM formatting
                privateKeyPem = NormalizePrivateKeyPem(privateKeyPem);

                // Parse the private key
                using (var reader = new StringReader(privateKeyPem))
                {
                    var pemReader = new PemReader(reader);
                    var privateKeyObject = pemReader.ReadObject();

                    // Handle different key formats
                    AsymmetricKeyParameter privateKey;
                    if (privateKeyObject is AsymmetricCipherKeyPair keyPair)
                    {
                        privateKey = keyPair.Private;
                    }
                    else if (privateKeyObject is ECPrivateKeyParameters ecPrivateKey)
                    {
                        privateKey = ecPrivateKey;
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported private key format");
                    }

                    // Convert hash from Base64
                    byte[] hashBytes = Convert.FromBase64String(hashBase64);

                    // Create ECDSA signature
                    ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
                    signer.Init(true, privateKey);
                    signer.BlockUpdate(hashBytes, 0, hashBytes.Length);

                    return signer.GenerateSignature();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Signature Generation Error: {ex.Message}");
                throw;
            }
        }

        public static bool VerifySignature(byte[] dataHash, byte[] signature, string certificateBase64)
        {
            try
            {
                Console.WriteLine("Debug - Hash (Hex): " + BitConverter.ToString(dataHash).Replace("-", ""));
                Console.WriteLine("Debug - Hash (Base64): " + Convert.ToBase64String(dataHash));
                Console.WriteLine("Debug - Signature Length: " + signature.Length);
                Console.WriteLine("Debug - Certificate Length: " + certificateBase64.Length);

                // Convert Base64 certificate to X509Certificate
                byte[] certBytes = Convert.FromBase64String(certificateBase64);
                var x509Cert = new X509Certificate2(certBytes);

                // Extract public key using BouncyCastle
                using (var reader = new StringReader(ConvertX509ToPem(x509Cert)))
                {
                    var pemReader = new PemReader(reader);
                    var cert = (Org.BouncyCastle.X509.X509Certificate)pemReader.ReadObject();

                    // Get public key
                    AsymmetricKeyParameter publicKey = cert.GetPublicKey();

                    // Verify signature
                    ISigner verifier = SignerUtilities.GetSigner("SHA-256withECDSA");
                    verifier.Init(false, publicKey);
                    verifier.BlockUpdate(dataHash, 0, dataHash.Length);

                    return verifier.VerifySignature(signature);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Detailed Verification Error: {ex.GetType().Name}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine($"Signature Verification Error: {ex.Message}");
                return false;
            }
        }

        // Helper method to normalize private key PEM
        private static string NormalizePrivateKeyPem(string privateKeyPem)
        {
            privateKeyPem = privateKeyPem.Trim();

            if (!privateKeyPem.Contains("-----BEGIN EC PRIVATE KEY-----"))
            {
                privateKeyPem = "-----BEGIN EC PRIVATE KEY-----\n" +
                                privateKeyPem +
                                "\n-----END EC PRIVATE KEY-----";
            }

            return privateKeyPem;
        }

        // Helper method to convert X509Certificate2 to PEM format
        private static string ConvertX509ToPem(X509Certificate2 certificate)
        {
            var certBytes = certificate.Export(X509ContentType.Cert);
            return "-----BEGIN CERTIFICATE-----\n" +
                   Convert.ToBase64String(certBytes, Base64FormattingOptions.InsertLineBreaks) +
                   "\n-----END CERTIFICATE-----";
        }

        public static string GeneratePfxBase64(string privateKeyPem, string certificateBase64, string? password)
        {
            // Convert Base64 string to byte array for the certificate
            byte[] certBytes = Convert.FromBase64String(certificateBase64);
            var certificate = new X509Certificate2(certBytes);

            // Convert PEM private key to DER byte array
            byte[] privateKeyBytes = ConvertPemToDer(privateKeyPem);

            try
            {
                using (var ec = ECDsa.Create())
                {
                    // Import EC private key
                    ec.ImportECPrivateKey(privateKeyBytes, out _);

                    // Combine certificate with private key
                    var certWithKey = certificate.CopyWithPrivateKey(ec);

                    // Export the PFX to Base64
                    return ExportToPfxBase64(certWithKey, password);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to import private key.", ex);
            }
        }

        private static string ExportToPfxBase64(X509Certificate2 certWithKey, string? password)
        {
            // Export the certificate with the private key to PFX
            var pfxBytes = certWithKey.Export(X509ContentType.Pfx, password);

            // Convert PFX to Base64 string
            return Convert.ToBase64String(pfxBytes);
        }

        private static byte[] ConvertPemToDer(string pem)
        {
            // Remove the header and footer from PEM format
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




