using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using NekitCoinsManager.Models;

namespace NekitCoinsManager.HttpClients
{
    /// <summary>
    /// Базовый класс для HTTP клиентов API
    /// </summary>
    public abstract class BaseApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonOptions;
        
        protected BaseApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }
        

        protected async Task<(bool success, string? error, T? data)> GetAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var data = await ProcessResponseAsync<T>(response);
                    return (true, null, data);
                }
                else
                {
                    var errorMessage = await GetErrorMessageAsync(response);
                    return (false, errorMessage, default);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, default);
            }
        }

        protected async Task<(bool success, string? error, TResponse? data)> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await ProcessResponseAsync<TResponse>(response);
                    return (true, null, result);
                }
                else
                {
                    var errorMessage = await GetErrorMessageAsync(response);
                    return (false, errorMessage, default);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, default);
            }
        }
        
        protected async Task<(bool success, string? error)> PostAsync<TRequest>(string url, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    var errorMessage = await GetErrorMessageAsync(response);
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        
        protected async Task<(bool success, string? error)> DeleteAsync(string url)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    var errorMessage = await GetErrorMessageAsync(response);
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        
        private async Task<T?> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
                return default;
                
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
                return default;
                
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
        protected async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrEmpty(content))
                    return GetDefaultErrorMessage(response.StatusCode);
                
                try
                {
                    var error = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);
                    if (!string.IsNullOrEmpty(error?.Error))
                        return error.Error;
                }
                catch
                {
                    if (!string.IsNullOrEmpty(content))
                        return content;
                }
                
                return GetDefaultErrorMessage(response.StatusCode);
            }
            catch
            {
                return GetDefaultErrorMessage(response.StatusCode);
            }
        }
        
        private string GetDefaultErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => "Неверный запрос",
                HttpStatusCode.Unauthorized => "Требуется авторизация",
                HttpStatusCode.Forbidden => "Доступ запрещен",
                HttpStatusCode.NotFound => "Ресурс не найден",
                HttpStatusCode.InternalServerError => "Внутренняя ошибка сервера",
                _ => $"Ошибка HTTP: {(int)statusCode}"
            };
        }
    }
}
