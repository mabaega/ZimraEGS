using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ZimraEGS.ApiClient.Models;

namespace ZimraEGS.ApiClient.Helpers
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiHelper(PlatformType environmentType)
        {
            // Set the base URL for API requests
            var baseUrl = Utilities.GetBaseUrl(environmentType);
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl.TrimEnd('/') : baseUrl;
            _httpClient = new HttpClient();
        }

        public ApiHelper(string pfxBase64, PlatformType environmentType)
        {
            // Explicitly set security protocols with more flexibility
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var baseUrl = Utilities.GetBaseUrl(environmentType);
            _baseUrl = baseUrl.EndsWith("/") ? baseUrl.TrimEnd('/') : baseUrl;

            try
            {
                var handler = new HttpClientHandler
                {
                    // More permissive SSL/TLS settings
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                    {
                        // Log any SSL policy errors for debugging
                        if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
                        {
                            Console.WriteLine($"SSL Policy Errors: {sslPolicyErrors}");
                        }
                        return true; // Allow all certificates for now (not recommended for production)
                    },

                    // Explicitly support TLS 1.2 and 1.3
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13
                };

                try
                {
                    // Load the certificate from Base64 PFX
                    var certificate = LoadCertificateFromPfx(pfxBase64, null); // Assuming no password

                    handler.ClientCertificates.Add(certificate);

                    _httpClient = new HttpClient(handler)
                    {
                        Timeout = TimeSpan.FromSeconds(30)
                    };
                }
                catch (Exception certEx)
                {
                    Console.WriteLine($"Certificate Loading Error: {certEx.Message}");
                    throw; // Rethrow to handle it upstream if necessary
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TLS Setup Error: {ex.Message}");
                throw; // Rethrow to handle it upstream if necessary
            }
        }

        private X509Certificate2 LoadCertificateFromPfx(string pfxBase64, string? password)
        {
            // Convert Base64 string to byte array
            byte[] pfxBytes = Convert.FromBase64String(pfxBase64);

            // Load the certificate from the PFX byte array
            var certificate = new X509Certificate2(pfxBytes, password, X509KeyStorageFlags.UserKeySet);

            return certificate;
        }

        // Example method to send GET request to the API
        public async Task<ServerResponse> SendGetRequestAsync(string lastPath, int? deviceID, string deviceModelName, string deviceModelVersion)
        {
            if (string.IsNullOrWhiteSpace(lastPath) || !deviceID.HasValue)
                throw new ArgumentException("Invalid arguments provided for the API request.");

            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/{lastPath}";

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendGetRequestAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        // Specific API Method for Verifying Taxpayer Information
        public async Task<ServerResponse> VerifyTaxpayerInformationAsync(int deviceID, VerifyTaxpayerInformationRequest body)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Public/v1/{deviceID}/VerifyTaxpayerInformation";

            // Serialize the request body to JSON using Newtonsoft.Json
            string requestBody = JsonConvert.SerializeObject(body);

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }
        public async Task<ServerResponse> RegisterDeviceAsync(int? deviceID, string deviceModelName, string deviceModelVersion, RegisterDeviceRequest body)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Public/v1/{deviceID}/RegisterDevice";

            // Serialize the request body to JSON using Newtonsoft.Json
            string requestBody = JsonConvert.SerializeObject(body);

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> GetServerCertificateAsync(string? thumbprint)
        {
            // Prepare the endpoint with query parameter
            string endpoint = $"{_baseUrl}/Public/v1/GetServerCertificate";
            endpoint += string.IsNullOrEmpty(thumbprint) ? "" : $"?thumbprint={thumbprint}";

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint)
            {
                // Add the Accept header to specify the response type (JSON)
                Headers = { { "accept", "application/json" } }
            };

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> IssueCertificateAsync(int? deviceID, string deviceModelName, string deviceModelVersion, IssueCertificateRequest body)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/IssueCertificate";

            // Serialize the request body to JSON using Newtonsoft.Json
            string requestBody = JsonConvert.SerializeObject(body);

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> GetConfigAsync(int? deviceID, string deviceModelName, string deviceModelVersion)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/GetConfig";

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> GetStatusAsync(int? deviceID, string deviceModelName, string deviceModelVersion)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/GetStatus";

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, endpoint);

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> PingAsync(int? deviceID, string deviceModelName, string deviceModelVersion)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/Ping";

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> OpenDayAsync(int? deviceID, string deviceModelName, string deviceModelVersion, OpenDayRequest body)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/OpenDay";

            // Serialize the request body to JSON using Newtonsoft.Json
            string requestBody = JsonConvert.SerializeObject(body);

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> SubmitReceiptAsync(int? deviceID, string deviceModelName, string deviceModelVersion, Receipt body)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/SubmitReceipt";

            // Serialize the request body to JSON using Newtonsoft.Json
            string requestBody = JsonConvert.SerializeObject(body);

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> CloseDayAsync(int? deviceID, string deviceModelName, string deviceModelVersion, OpenDayRequest body)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/CloseDay";

            // Serialize the request body to JSON using Newtonsoft.Json
            string requestBody = JsonConvert.SerializeObject(body);

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

        public async Task<ServerResponse> SendPostRequestAsync<TBody>(string lastPath, int? deviceID, string deviceModelName, string deviceModelVersion, TBody? body)
        {
            // Prepare the endpoint
            string endpoint = $"{_baseUrl}/Device/v1/{deviceID}/{lastPath}";


            // Serialize the request body to JSON
            string requestBody = JsonConvert.SerializeObject(body);

            // Create the HttpRequestMessage
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);

            if (body != null)
            {
                requestMessage.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            };

            // Add headers to the request
            requestMessage.Headers.Add("DeviceModelName", deviceModelName);
            requestMessage.Headers.Add("DeviceModelVersion", deviceModelVersion);

            // Send the request and get the response
            try
            {
                using var response = await _httpClient.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Return the ServerResponse
                return new ServerResponse(
                    endpoint,
                    response.StatusCode,
                    response.Headers,
                    responseContent
                );
            }
            catch (Exception ex)
            {
                // Handle errors and exceptions
                throw new Exception($"An error occurred while calling the API: {ex.Message}", ex);
            }
        }

    }
}