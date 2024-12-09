using Newtonsoft.Json;
using System.Text;
using Zimra.ApiClient.Models;

namespace Zimra.ApiClient.Helpers
{
    public static class Utilities
    {
        public static void PrepareRequest(HttpRequestMessage request, string url)
        {
            // Log URL
            Console.WriteLine($"Request URL: {url}");

            // Log headers
            Console.WriteLine("Request Headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
            }

            // Log request body (Jika ada)
            if (request.Content != null)
            {
                var content = request.Content.ReadAsStringAsync().Result;  // Dapatkan body request
                Console.WriteLine($"Request Body: {content}");
            }
        }

        public static void PrepareRequest(HttpRequestMessage request, StringBuilder urlBuilder)
        {
            // Log URL yang dibangun
            Console.WriteLine($"Built Request URL: {urlBuilder}");

            // Log headers
            Console.WriteLine("Request Headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(",", header.Value)}");
            }

            // Log request body (Jika ada)
            if (request.Content != null)
            {
                var content = request.Content.ReadAsStringAsync().Result;  // Dapatkan body request
                Console.WriteLine($"Request Body: {content}");
            }
        }

        public static void ProcessResponse(HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                // Baca respons JSON
                var responseJson = response.Content.ReadAsStringAsync().Result;

                // Format JSON agar lebih mudah dibaca
                var formattedJson = JsonConvert.SerializeObject(
                    JsonConvert.DeserializeObject(responseJson),
                    Formatting.Indented
                );

                Console.WriteLine("Response JSON (Formatted):");
                Console.WriteLine(formattedJson);
            }
        }

        public static JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            UpdateJsonSerializerSettings(settings);
            return settings;
        }

        private static void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.DateFormatString = "yyyy-MM-dd";
        }

        public static async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, bool readResponseAsString, JsonSerializerSettings jsonSerializerSettings, CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }

            if (readResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var typedBody = JsonConvert.DeserializeObject<T>(responseText, jsonSerializerSettings);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                    using var streamReader = new StreamReader(responseStream);
                    using var jsonTextReader = new JsonTextReader(streamReader);
                    var serializer = JsonSerializer.Create(jsonSerializerSettings);
                    var typedBody = serializer.Deserialize<T>(jsonTextReader);
                    return new ObjectResponseResult<T>(typedBody, string.Empty);
                }
                catch (JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }
        public static string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }

            if (value is Enum)
            {
                var name = Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        if (System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) is System.Runtime.Serialization.EnumMemberAttribute attribute)
                        {
                            return attribute.Value ?? name;
                        }
                    }

                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted ?? string.Empty;
                }
            }
            else if (value is bool b)
            {
                return Convert.ToString(b, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[] v)
            {
                return Convert.ToBase64String(v);
            }
            else if (value.GetType().IsArray)
            {
                var array = ((Array)value).OfType<object>();
                return string.Join(",", array.Select(o => ConvertToString(o, cultureInfo)));
            }

            var result = Convert.ToString(value, cultureInfo);
            return result ?? "";
        }

        public static async Task<TResponse> HandleResponseAsync<TResponse>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers, bool readResponseAsString, JsonSerializerSettings jsonSerializerSettings, CancellationToken cancellationToken)
        {
            var status = (int)response.StatusCode;

            if (status == 200)
            {
                var objectResponse = await ReadObjectResponseAsync<TResponse>(
                    response, headers, readResponseAsString, jsonSerializerSettings, cancellationToken).ConfigureAwait(false);

                if (objectResponse.Object == null)
                {
                    throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                }
                return objectResponse.Object;
            }

            if (status == 400)
            {
                var objectResponse = await ReadObjectResponseAsync<ValidationProblemDetails>(
                    response, headers, readResponseAsString, jsonSerializerSettings, cancellationToken).ConfigureAwait(false);

                if (objectResponse.Object == null)
                {
                    throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                }
                throw new ApiException<ValidationProblemDetails>("Bad Request", status, objectResponse.Text, headers, objectResponse.Object, null);
            }
            else
            if (status == 401)
            {
                var objectResponse_ = await ReadObjectResponseAsync<ProblemDetails>(response, headers, readResponseAsString, jsonSerializerSettings, cancellationToken).ConfigureAwait(false);
                if (objectResponse_.Object == null)
                {
                    throw new ApiException("Response was null which was not expected.", status, objectResponse_.Text, headers, null);
                }
                throw new ApiException<ProblemDetails>("Device certificate expired.", status, objectResponse_.Text, headers, objectResponse_.Object, null);
            }
            else
            if (status == 404) //product
            {
                string responseText_ = response.Content == null ? string.Empty : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new ApiException("Operation failed because no device were found with provided device id.", status, responseText_, headers, null);
            }
            else
            if (status == 404)
            {
                var objectResponse = await ReadObjectResponseAsync<ProblemDetails>(
                    response, headers, readResponseAsString, jsonSerializerSettings, cancellationToken).ConfigureAwait(false);

                if (objectResponse.Object == null)
                {
                    throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                }
                throw new ApiException<ProblemDetails>("Certificate requested by thumbprint not found", status, objectResponse.Text, headers, objectResponse.Object, null);
            }

            if (status == 422)
            {
                var objectResponse = await ReadObjectResponseAsync<ApiProblemDetails>(
                    response, headers, readResponseAsString, jsonSerializerSettings, cancellationToken).ConfigureAwait(false);

                if (objectResponse.Object == null)
                {
                    throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                }
                throw new ApiException<ApiProblemDetails>("Operation failed because of provided data or invalid object state in Fiscal backend.", status, objectResponse.Text, headers, objectResponse.Object, null);
            }

            if (status == 500)
            {
                var objectResponse = await ReadObjectResponseAsync<ProblemDetails>(
                    response, headers, readResponseAsString, jsonSerializerSettings, cancellationToken).ConfigureAwait(false);

                if (objectResponse.Object == null)
                {
                    throw new ApiException("Response was null which was not expected.", status, objectResponse.Text, headers, null);
                }
                throw new ApiException<ProblemDetails>("Server encountered temporary issues.", status, objectResponse.Text, headers, objectResponse.Object, null);
            }

            var responseData = response.Content == null ? null : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new ApiException("The HTTP status code of the response was not expected (" + status + ").", status, responseData, headers, null);
        }

    }

}

