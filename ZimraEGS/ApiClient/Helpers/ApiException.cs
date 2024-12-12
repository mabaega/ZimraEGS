namespace ZimraEGS.ApiClient.Helpers
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    // Kelas ApiException generik untuk menangani respons API
    public class ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException) : Exception(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + FormatJson(response), innerException)
    {
        public int StatusCode { get; private set; } = statusCode;
        public string Response { get; private set; } = response;
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; } = headers;

        private static string FormatJson(string json)
        {
            try
            {
                var parsedJson = JsonConvert.DeserializeObject(json);
                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch
            {
                // Jika gagal memformat, kembalikan JSON asli
                return json;
            }
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    // Kelas ApiException generik dengan parameter TResult untuk menangani hasil
    public class ApiException<TResult>(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException) : ApiException(message, statusCode, response, headers, innerException)
    {
        public TResult Result { get; private set; } = result;
    }
}
