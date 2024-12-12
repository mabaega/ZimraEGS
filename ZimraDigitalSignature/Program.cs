using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class CryptoHelper
{
    // Compute SHA-256 hash
    public static byte[] ComputeSHA256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
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

    // Sign the hash with ECC private key
    public static byte[] SignDocument(ECDsa privateKey, byte[] hash)
    {
        return privateKey.SignHash(hash);
    }

    // Verify the signature with ECC public key
    public static bool VerifySignature(X509Certificate2 deviceCertificate, byte[] hash, byte[] signature)
    {
        using (ECDsa publicKey = deviceCertificate.GetECDsaPublicKey())
        {
            if (publicKey == null)
                throw new InvalidOperationException("Could not extract public key from certificate");

            return publicKey.VerifyHash(hash, signature);
        }
    }
}

public class Program
{
    public static void Main()
    {
        // base64string PrivateKey and Device Certificate
        string privateKeyBase64 = "MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQg0BCJ+P4SHbSjiX7EAeDL8BFnPFxR1z7J++g0ZuSj+gyhRANCAARj3CRhavSIZPuJ7gBUUs61wXRdagAkIGkRdYwTCYv1XnL94R/tVPN27pw9u6N5sXCvx2zvoaMu+fB9vvdf2spi";
        string deviceCertificateBase64 = "MIIDWjCCAkKgAwIBAgIIGSLBMKhNwNcwDQYJKoZIhvcNAQELBQAwVDELMAkGA1UEBhMCTFQxEDAOBgNVBAgMB1ZpbG5pdXMxEDAOBgNVBAcMB1ZpbG5pdXMxDjAMBgNVBAoMBVpJTVJBMREwDwYDVQQDDAhaSU1SQV9DQTAeFw0yNDEyMTAxNjM2MzhaFw0yNzEyMTAxNjM2MzhaMGgxCzAJBgNVBAYTAlpXMREwDwYDVQQIDAhaaW1iYWJ3ZTEjMCEGA1UECgwaWmltYmFid2UgUmV2ZW51ZSBBdXRob3JpdHkxITAfBgNVBAMMGFpJTVJBLU1HUjI0MDEtMDAwMDAyMDIwMzBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABGPcJGFq9Ihk+4nuAFRSzrXBdF1qACQgaRF1jBMJi/Vecv3hH+1U83bunD27o3mxcK/HbO+hoy758H2+91/aymKjgeYwgeMwCQYDVR0TBAIwADAdBgNVHQ4EFgQUsUo8tPaxScr5PMGh5RM3SV8B+qkwgZEGA1UdIwSBiTCBhoAUU7/avL3rxixSYklqUei9iWSpTjahWKRWMFQxCzAJBgNVBAYTAkxUMRAwDgYDVQQIDAdWaWxuaXVzMRAwDgYDVQQHDAdWaWxuaXVzMQ4wDAYDVQQKDAVaSU1SQTERMA8GA1UEAwwIWklNUkFfQ0GCFHDBnrsbY/FDI3iqezfqBp0Wqo5gMA4GA1UdDwEB/wQEAwIF4DATBgNVHSUEDDAKBggrBgEFBQcDAjANBgkqhkiG9w0BAQsFAAOCAQEAuPfa6G9+8UBki56rZIHyH9+yfXuYPGaxpl63wM+gMgteE/MhlF3Jp/7NgISASkgZMMCXDNaN80DeJUQz6c+i+WFfh3wQCnz9slnsulnaVZjFW4Ik/0CFZA//PCLB3bKXoW4zYPNh5Vfm82NFsRxk4Zby/4JOSZRObagjX4EgSYyCoV8FmROU0KWuZZL7HUJtryMRrMljGyFdMCo/QXhxyCgviiLEDStB/jChTbH001xBK+HxXewez45DBFqhOrlMNkoA1J++hSUnM+pzZqotDP2J1FSJ9cgV2Jef6sOLypC4tTp4cKS0cwERCNQTZnpeyl1vMi5IVdzIUqv1rqWYgA==";

        // Define the inputs
        string deviceID = "322";
        string receiptType = "FISCALINVOICE";
        string receiptCurrency = "USD";
        string receiptGlobalNo = "85";
        string receiptDate = "2020-09-19T09:23:07";
        string receiptTotal = (40.35 * 100).ToString("F0"); // Convert to cents
        string receiptTaxsHash = "07000.000100014.50535";
        string previousReceiptHash = "hNVJXP/ACOiE8McD3pKsDlqBXpuaUqQOfPnMyfZWI9k=";

        // Create the concatenated string for hash generation
        string sourcesHash = (deviceID + 
            receiptType.ToUpper() + 
            receiptCurrency + 
            receiptGlobalNo + 
            receiptDate + 
            receiptTotal + 
            receiptTaxsHash);

        if (!string.IsNullOrEmpty(previousReceiptHash))
        {
            sourcesHash += previousReceiptHash;
        }

        Console.WriteLine("Sources Hash String: " + sourcesHash);

        // Generate hash
        byte[] hashByte = CryptoHelper.ComputeSHA256Hash(sourcesHash);
        Console.WriteLine("Hash (Base64): " + Convert.ToBase64String(hashByte));

        // Load the private key and device certificate
        ECDsa privateKey = CryptoHelper.ConvertPrivateKeyFromBase64(privateKeyBase64);
        X509Certificate2 deviceCertificate = CryptoHelper.ConvertCertificateFromBase64(deviceCertificateBase64);

        // Generate signature
        byte[] signatureByte = CryptoHelper.SignDocument(privateKey, hashByte);
        Console.WriteLine("Signature (Base64): " + Convert.ToBase64String(signatureByte));

        // Set signature data
        SignatureData receiptDeviceSignature = new SignatureData()
        {
            Hash = hashByte,
            Signature = signatureByte ?? Encoding.UTF8.GetBytes("")
        };

        // Verify signature
        bool isValid = CryptoHelper.VerifySignature(deviceCertificate, hashByte, signatureByte);

        if (isValid)
        {
            Console.WriteLine("Signature is valid.");
        }
        else
        {
            Console.WriteLine("Signature is invalid.");
            Console.WriteLine("Hash (Base64): " + Convert.ToBase64String(receiptDeviceSignature.Hash));
            Console.WriteLine("Signature (Base64): " + Convert.ToBase64String(receiptDeviceSignature.Signature));
            Console.WriteLine("Certificate: " + deviceCertificate);
        }
    }
}

public class SignatureData
{
    public byte[] Hash { get; set; }
    public byte[] Signature { get; set; }
}
