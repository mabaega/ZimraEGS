using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace ZimraEGS.Helpers
{
    public static class Utils
    {
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
        public static string SerializeObject<T>(T value)
        {
            StringBuilder sb = new StringBuilder(256);
            StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            var jsonSerializer = JsonSerializer.CreateDefault();
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonWriter.IndentChar = ' ';
                jsonWriter.Indentation = 4;

                jsonSerializer.Serialize(jsonWriter, value, typeof(T));
            }

            return sw.ToString();
        }

    }
}
