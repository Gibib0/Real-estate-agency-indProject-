using Integration.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Integration
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7030";

        public AuthService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            _httpClient = new HttpClient(handler);
        }

        public async Task<LoginResponse> Login(string username, string password)
        {
            var loginRequest = new LoginRequest { Username = username, Password = password };
            string apiUrl = $"{_apiBaseUrl}/api/users/login";

            try
            {
                var response = await _httpClient.PostAsJsonAsync(apiUrl, loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                    if (loginResponse != null)
                    {
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);
                    }

                    return loginResponse;
                }
                else
                {
                    Console.WriteLine($"Server error: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Network error: {ex.Message}");
                return null;
            }
        }
    }
}