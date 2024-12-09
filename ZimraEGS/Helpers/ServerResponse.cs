using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ZimraEGS.Helpers
{
    public class ServerResponse
    {
        public string Endpoint { get; }
        public HttpStatusCode StatusCode { get; }
        public HttpResponseHeaders Headers { get; }
        public string ResponseContent { get; } = "{}";

        public ServerResponse(string endpoint, HttpStatusCode statusCode, HttpResponseHeaders headers, string responseContent)
        {
            Endpoint = endpoint;
            StatusCode = statusCode;
            Headers = headers;

            // Ensure ResponseContent is not serialized again if it's already a JSON string
            ResponseContent = responseContent;
        }

        // Deserialize the response content into a specific object using Newtonsoft.Json
        public T GetContentAs<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(ResponseContent);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error deserializing response content: {ex.Message}", ex);
            }
        }

        // Get the raw response content as a string
        public string GetContentAsString()
        {
            return ResponseContent;
        }

        // Returns the full response as a JSON string (for logging or debugging)
        public string GetFullResponseAsJson()
        {
            var responseLog = new
            {
                Endpoint,
                StatusCode = (int)StatusCode,
                StatusDescription = StatusCode.ToString(),
                // Deserialize ResponseContent to ensure it's treated as an object
                ResponseContent = JsonConvert.DeserializeObject(ResponseContent)
            };

            return JsonConvert.SerializeObject(responseLog, Formatting.Indented);
        }
    }

}