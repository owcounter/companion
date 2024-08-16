using Owcounter.Authentication;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Owcounter.Services
{
    public class ApiService
    {
        private readonly HttpClient httpClient;
        private readonly KeycloakAuth keycloakAuth;
        private readonly string tokenFileName;
        private string? accessToken;
        private string? refreshToken;
        private readonly Action<string> log;

        public ApiService(string apiBaseUrl, KeycloakAuth keycloakAuth, string tokenFileName, Action<string> log)
        {
            httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
            this.keycloakAuth = keycloakAuth;
            this.tokenFileName = tokenFileName;
            this.log = log;
        }

        public async Task<bool> LoadAndValidateTokens()
        {
            if (!System.IO.File.Exists(tokenFileName))
                return false;

            try
            {
                var tokenJson = await System.IO.File.ReadAllTextAsync(tokenFileName);
                var tokenResponse = JsonSerializer.Deserialize<KeycloakConnectOutput>(tokenJson);
                accessToken = tokenResponse?.access_token;
                refreshToken = tokenResponse?.refresh_token;

                if (accessToken != null && await ValidateToken(accessToken))
                    return true;

                if (await RefreshAndValidateToken())
                    return true;
            }
            catch (Exception ex)
            {
                log($"Error loading or validating tokens: {ex.Message}");
            }

            DeleteTokenFile();
            return false;
        }

        private async Task<bool> ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await httpClient.GetAsync("/user/session");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                log($"Token validation failed: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> RefreshAndValidateToken()
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                    return false;

                var tokenResponse = await keycloakAuth.RefreshToken(refreshToken);
                accessToken = tokenResponse.access_token;
                refreshToken = tokenResponse.refresh_token;

                await System.IO.File.WriteAllTextAsync(tokenFileName, JsonSerializer.Serialize(tokenResponse));
                log("Token refreshed successfully");

                return accessToken != null && await ValidateToken(accessToken);
            }
            catch (Exception ex)
            {
                log($"Failed to refresh token: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendScreenshotToServer(string screenshotBase64)
        {
            string formattedBase64 = $"data:image/jpeg;base64,{screenshotBase64}";
            var input = new { screenshot_base64 = formattedBase64 };
            var content = new StringContent(JsonSerializer.Serialize(input), System.Text.Encoding.UTF8, "application/json");

            return await SendApiRequestWithRetry(() => httpClient.PostAsync("/process-screenshot", content));
        }

        private async Task<bool> SendApiRequestWithRetry(Func<Task<HttpResponseMessage>> apiCall)
        {
            int maxRetries = 2;
            for (int i = 0; i <= maxRetries; i++)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                try
                {
                    var response = await apiCall();
                    if (response.IsSuccessStatusCode)
                    {
                        log("API request processed successfully");
                        return true;
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    log($"Error processing API request. Status code: {response.StatusCode}. Content: {errorContent}");

                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
                    if (errorResponse?.code == 2 && errorResponse?.message?.Contains("ERR_NO_WEBSOCKET_OPENED") == true)
                    {
                        log("Please refresh your Owcounter website to reconnect the WebSocket.");
                        return false;
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || errorContent.Contains("Token is expired"))
                    {
                        if (i < maxRetries && await RefreshAndValidateToken())
                        {
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    log($"HTTP Request failed: {e.Message}");
                }

                if (i == maxRetries)
                {
                    log("Max retries reached. Please check your connection or log in again.");
                    return false;
                }
            }

            return false;
        }

        private void DeleteTokenFile()
        {
            if (System.IO.File.Exists(tokenFileName))
            {
                System.IO.File.Delete(tokenFileName);
                log("Invalid tokens deleted. Please log in again.");
            }
        }

        public async Task Logout()
        {
            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                    await keycloakAuth.RevokeToken(accessToken);
                log("Logged out successfully");
            }
            catch (Exception ex)
            {
                log($"Error during logout: {ex.Message}");
            }
            finally
            {
                DeleteTokenFile();
                accessToken = null;
                refreshToken = null;
            }
        }

        public class ErrorResponse
        {
            public int code { get; set; }
            public string? message { get; set; }
        }
    }
}