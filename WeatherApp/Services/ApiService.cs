namespace WeatherApp.Services
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;
    using WeatherApp.Models;

    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://4m6455np-7299.uks1.devtunnels.ms/";
        private readonly ILogger<ApiService> _logger;

        JsonSerializerOptions _serializerOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
               PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<ApiResponse<bool>> RegisterUser(string name, string email, string phoneNumber, string password)
        {
            try
            {
                var register = new Register
                {
                    Name = name,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Password = password,
                };

                var json = JsonSerializer.Serialize(register, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Register", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");

                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                    };
                }

                return new ApiResponse<bool>
                {
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering user: {ex.Message}");

                return new ApiResponse<bool>
                {
                    ErrorMessage = ex.Message,
                };
            }
        }

        public async Task<ApiResponse<bool>> Login(string email, string password)
        {
            try
            {
                var login = new Login
                {
                    Email = email,
                    Password = password,
                };

                var json = JsonSerializer.Serialize(login, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await PostRequest("api/Users/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");

                    return new ApiResponse<bool>
                    {
                        ErrorMessage = $"Error sending HTTP request: {response.StatusCode}"
                    };
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);

                Preferences.Set("accesstoken", result!.AccessToken);
                Preferences.Set("userid", (int)result.UserId!);
                Preferences.Set("username", result.UserName);

                return new ApiResponse<bool>
                {
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging in: {ex.Message}");

                return new ApiResponse<bool>
                {
                    ErrorMessage = ex.Message,
                };
            }
        }

        public async Task<ApiResponse<bool>> UploadUserImage(byte[] imageArray)
        {
            try
            {
                var content = new MultipartFormDataContent
                {
                    { new ByteArrayContent(imageArray), "image", "image.jpg" }
                };
               
                var response = await PostRequest("api/Users/uploadphoto", content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = response.StatusCode == HttpStatusCode.Unauthorized
                        ? "Unauthorized"
                        : $"Error sending HTTP request: {response.StatusCode}";

                    _logger.LogError($"Error sending HTTP request: {response.StatusCode}");

                    return new ApiResponse<bool>
                    {
                        ErrorMessage = errorMessage,
                    };
                }

                return new ApiResponse<bool>
                {
                    Data = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading user's image: {ex.Message}");

                return new ApiResponse<bool>
                {
                    ErrorMessage = ex.Message,
                };
            }
        }

        private async Task<HttpResponseMessage> PostRequest(string uri, HttpContent content)
        {
            var urlAddress = _baseUrl + uri;

            try
            {
                var result = await _httpClient.PostAsync(urlAddress, content);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending POST request to {uri}: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<(City? CityDetails, string? ErrorMessage)> GetCityDetails(int cityId)
        {
            string endpoint = $"api/cities/{cityId}";
            return await GetAsync<City>(endpoint);
        }

        public async Task<(ProfileImage? ProfileImage, string? ErrorMessage)> GetUserProfileImage()
        {
            string endpoint = "api/Users/profileimage";
            return await GetAsync<ProfileImage>(endpoint);
        }

        private async Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint)
        {
            try
            {
                AddAuthorizationHeader();

                var response = await _httpClient.GetAsync(_baseUrl + endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);

                    return (data ?? Activator.CreateInstance<T>(), null);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string errorMessage = "Unauthorized";
                        _logger.LogWarning(errorMessage);
                        return (default, errorMessage);
                    }

                    string generalErrorMessage = $"Request error: {response.ReasonPhrase}";
                    _logger.LogError(generalErrorMessage);
                    return (default, generalErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = $"HTTP request error: {ex.Message}";
                _logger.LogError(errorMessage);
                return (default, errorMessage);
            }
            catch (JsonException ex)
            {
                string errorMessage = $"JSON deserialization error: {ex.Message}";
                _logger.LogError(errorMessage);
                return (default, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Unexpected error: {ex.Message}";
                _logger.LogError(errorMessage);
                return (default, errorMessage);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = Preferences.Get("accesstoken", string.Empty);

            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);          
        }
    }
}
