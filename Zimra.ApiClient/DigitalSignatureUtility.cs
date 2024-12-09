namespace Zimra.ApiClient
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class DigitalSignatureUtility
    {
        public static byte[] ComputeHash(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
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

        public static string ComputeMD5(string dataString)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(dataString));
                return BitConverter.ToString(hash).Replace("-", "").ToUpper().Substring(0, 16);
            }
        }

        public static byte[] GenerateSignature(byte[] hash, string cleanBase64PrivateKey)
        {
            //Console.WriteLine(cleanBase64PrivateKey);
            byte[] privateKey = ParsePrivateKey(cleanBase64PrivateKey);
            byte[] signature = Sign(hash, privateKey);
            return signature;
        }
        public static byte[] GenerateSignature(string dataToSign, string cleanBase64PrivateKey)
        {
            byte[] privateKey = ParsePrivateKey(cleanBase64PrivateKey);
            byte[] hash = ComputeHash(dataToSign);
            byte[] signature = Sign(hash, privateKey);
            return signature;
        }
        public static bool VerifySignature(byte[] signature, byte[] hash, string cleanBase64PrivateKey)
        {
            byte[] privateKey = ParsePrivateKey(cleanBase64PrivateKey);
            byte[] publicKey = ExtractPublicKey(privateKey);
            return Verify(hash, signature, publicKey);
        }
        public static bool VerifySignature(byte[] signature, string data, string cleanBase64PrivateKey)
        {
            byte[] privateKey = ParsePrivateKey(cleanBase64PrivateKey);
            byte[] publicKey = ExtractPublicKey(privateKey);
            byte[] hash = ComputeHash(data);
            return Verify(hash, signature, publicKey);
        }

        private static byte[] ParsePrivateKey(string pemPrivateKey)
        {
            string base64Key = Regex.Replace(pemPrivateKey, @"-----BEGIN (EC|RSA|PRIVATE) KEY-----\n?|-----END (EC|RSA|PRIVATE) KEY-----\n?", "")
                                     .Replace("\n", "")
                                     .Replace("\r", "");

            byte[] keyBytes = Convert.FromBase64String(base64Key);

            byte[][] parsingStrategies = new byte[][]
            {
            keyBytes, // Original bytes
            StripPkcs8Header(keyBytes), // Strip PKCS8 header if present
            };

            foreach (var strategy in parsingStrategies)
            {
                if (TryParseECKey(strategy, out byte[] _))
                {
                    return strategy;
                }

                if (TryParseRSAKey(strategy, out byte[] _))
                {
                    return strategy;
                }
            }

            throw new ArgumentException("Unable to parse the private key. Unsupported key format.");
        }

        private static bool TryParseECKey(byte[] keyBytes, out byte[] parsedKey)
        {
            try
            {
                using (var ecdsa = ECDsa.Create())
                {
                    ecdsa.ImportPkcs8PrivateKey(keyBytes, out _);
                    parsedKey = keyBytes;
                    return true;
                }
            }
            catch
            {
                try
                {
                    using (var ecdsa = ECDsa.Create())
                    {
                        ecdsa.ImportECPrivateKey(keyBytes, out _);
                        parsedKey = keyBytes;
                        return true;
                    }
                }
                catch
                {
                    parsedKey = null;
                    return false;
                }
            }
        }
        private static bool TryParseRSAKey(byte[] keyBytes, out byte[] parsedKey)
        {
            try
            {
                using (var rsa = RSA.Create())
                {
                    rsa.ImportPkcs8PrivateKey(keyBytes, out _);
                    parsedKey = keyBytes;
                    return true;
                }
            }
            catch
            {
                try
                {
                    using (var rsa = RSA.Create())
                    {
                        rsa.ImportRSAPrivateKey(keyBytes, out _);
                        parsedKey = keyBytes;
                        return true;
                    }
                }
                catch
                {
                    parsedKey = null;
                    return false;
                }
            }
        }

        private static byte[] StripPkcs8Header(byte[] keyBytes)
        {
            // Basic PKCS8 header stripping logic
            if (keyBytes.Length > 30 && keyBytes[0] == 0x30) // Check for ASN.1 sequence
            {
                // Find the actual key data
                int headerLength = FindPkcs8HeaderEnd(keyBytes);

                // Extract the actual key bytes
                byte[] strippedKey = new byte[keyBytes.Length - headerLength];
                Array.Copy(keyBytes, headerLength, strippedKey, 0, strippedKey.Length);
                return strippedKey;
            }
            return keyBytes;
        }

        private static int FindPkcs8HeaderEnd(byte[] keyBytes)
        {
            // Very basic PKCS8 header parsing
            for (int i = 1; i < Math.Min(keyBytes.Length, 30); i++)
            {
                if (keyBytes[i] == 0x02 && keyBytes[i + 1] > 0 && keyBytes[i + 1] < 33)
                {
                    return i;
                }
            }
            return 0;
        }
        public static byte[] Sign(byte[] hash, byte[] privateKey)
        {
            try
            {
                // Try EC key first
                using (var ecdsa = ECDsa.Create())
                {
                    try
                    {
                        ecdsa.ImportPkcs8PrivateKey(privateKey, out _);
                        return ecdsa.SignHash(hash);
                    }
                    catch
                    {
                        ecdsa.ImportECPrivateKey(privateKey, out _);
                        return ecdsa.SignHash(hash);
                    }
                }
            }
            catch
            {
                // If EC fails, try RSA
                using (var rsa = RSA.Create())
                {
                    try
                    {
                        rsa.ImportPkcs8PrivateKey(privateKey, out _);
                        return rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    }
                    catch
                    {
                        rsa.ImportRSAPrivateKey(privateKey, out _);
                        return rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    }
                }
            }
        }
        public static bool Verify(byte[] hash, byte[] signature, byte[] publicKey)
        {
            try
            {
                // Try RSA verification first
                using (var rsa = RSA.Create())
                {
                    try
                    {
                        rsa.ImportRSAPublicKey(publicKey, out _);
                        return rsa.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    }
                    catch
                    {
                        // If RSA fails, try EC
                        using (var ecdsa = ECDsa.Create())
                        {
                            ecdsa.ImportSubjectPublicKeyInfo(publicKey, out _);
                            return ecdsa.VerifyHash(hash, signature);
                        }
                    }
                }
            }
            catch
            {
                // If both RSA and EC verification fail
                return false;
            }
        }

        public static byte[] ExtractPublicKey(byte[] privateKey)
        {
            try
            {
                // Try EC key first
                using (var ecdsa = ECDsa.Create())
                {
                    try
                    {
                        ecdsa.ImportPkcs8PrivateKey(privateKey, out _);
                        return ecdsa.ExportSubjectPublicKeyInfo();
                    }
                    catch
                    {
                        ecdsa.ImportECPrivateKey(privateKey, out _);
                        return ecdsa.ExportSubjectPublicKeyInfo();
                    }
                }
            }
            catch
            {
                // If EC fails, try RSA
                using (var rsa = RSA.Create())
                {
                    try
                    {
                        rsa.ImportPkcs8PrivateKey(privateKey, out _);
                        return rsa.ExportRSAPublicKey();
                    }
                    catch
                    {
                        rsa.ImportRSAPrivateKey(privateKey, out _);
                        return rsa.ExportRSAPublicKey();
                    }
                }
            }
        }
    }
}