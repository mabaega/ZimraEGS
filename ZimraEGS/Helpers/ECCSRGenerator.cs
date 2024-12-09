using System.Text;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;

namespace ZimraEGS.Helpers
{
    public class ECCSRGenerator
    {
        public (string csrPem, string privateKeyPem) GenerateCsrAndPrivateKey(string commonName)
        {
            // Generate ECC Key Pair (secp256r1)
            AsymmetricCipherKeyPair keyPair = GenerateEccKeyPair();

            // Create the CSR
            string csrPem = GenerateCsrPem(keyPair, commonName);

            // Get the private key in PEM format
            string privateKeyPem = GetPrivateKeyPem(keyPair);

            return (csrPem, privateKeyPem);
        }

        private AsymmetricCipherKeyPair GenerateEccKeyPair()
        {
            var parameters = new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, new SecureRandom());
            var keyPairGenerator = new ECKeyPairGenerator();
            keyPairGenerator.Init(parameters);
            return keyPairGenerator.GenerateKeyPair();
        }

        private string GenerateCsrPem(AsymmetricCipherKeyPair keyPair, string commonName)
        {
            var subject = new X509Name($"CN={commonName}");
            var csrGen = new Pkcs10CertificationRequest("SHA256withECDSA", subject, keyPair.Public, null, keyPair.Private);

            StringBuilder builder = new StringBuilder();
            PemWriter pemWriter = new PemWriter(new StringWriter(builder));
            pemWriter.WriteObject(csrGen);
            pemWriter.Writer.Flush();

            return builder.ToString();
        }

        private string GetPrivateKeyPem(AsymmetricCipherKeyPair keyPair)
        {
            StringBuilder builder = new StringBuilder();
            PemWriter pemWriter = new PemWriter(new StringWriter(builder));
            pemWriter.WriteObject(keyPair.Private);
            pemWriter.Writer.Flush();

            return builder.ToString();
        }
    }
}



