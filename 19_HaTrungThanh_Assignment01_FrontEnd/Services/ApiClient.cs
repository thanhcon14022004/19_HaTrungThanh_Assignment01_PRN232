using System.Net.Http.Json;
using System.Text.Json;
using _19_HaTrungThanh_Assignment01_FrontEnd.Models;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Services
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiClient(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("BackendApi");
        }

        // --- AUTH ---
        public async Task<(bool Success, LoginResponseModel? Data, string Error)> LoginAsync(LoginViewModel model)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("auth/login", model);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<LoginResponseModel>(_jsonOptions);
                    return (true, data, string.Empty);
                }
                var errObj = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                string errMsg = errObj.TryGetProperty("message", out var msg) ? msg.GetString() ?? "Login failed" : "Login failed";
                return (false, null, errMsg);
            }
            catch (Exception ex)
            {
                return (false, null, "Unable to connect to Backend API: " + ex.Message);
            }
        }

        // --- NEWS ARTICLES ---
        public async Task<List<NewsArticleViewModel>> GetActiveNewsArticlesAsync(string? keyword = null)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(keyword) ? "newsarticles/active" : $"newsarticles/active?keyword={Uri.EscapeDataString(keyword)}";
                return await _client.GetFromJsonAsync<List<NewsArticleViewModel>>(url, _jsonOptions) ?? new List<NewsArticleViewModel>();
            }
            catch
            {
                return new List<NewsArticleViewModel>();
            }
        }

        public async Task<List<NewsArticleViewModel>> GetNewsArticlesAsync(string? keyword = null)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(keyword) ? "newsarticles" : $"newsarticles?keyword={Uri.EscapeDataString(keyword)}";
                return await _client.GetFromJsonAsync<List<NewsArticleViewModel>>(url, _jsonOptions) ?? new List<NewsArticleViewModel>();
            }
            catch
            {
                return new List<NewsArticleViewModel>();
            }
        }

        public async Task<NewsArticleViewModel?> GetNewsArticleByIdAsync(string id)
        {
            try
            {
                return await _client.GetFromJsonAsync<NewsArticleViewModel>($"newsarticles/{id}", _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<NewsArticleViewModel>> GetNewsHistoryAsync(short creatorId)
        {
            try
            {
                return await _client.GetFromJsonAsync<List<NewsArticleViewModel>>($"newsarticles/history/{creatorId}", _jsonOptions) ?? new List<NewsArticleViewModel>();
            }
            catch
            {
                return new List<NewsArticleViewModel>();
            }
        }

        public async Task<(bool Success, string Message)> CreateNewsArticleAsync(NewsArticleViewModel model)
        {
            try
            {
                var payload = new
                {
                    model.NewsArticleID,
                    model.NewsTitle,
                    model.Headline,
                    model.NewsContent,
                    model.NewsSource,
                    model.CategoryID,
                    model.NewsStatus,
                    model.CreatedByID,
                    model.UpdatedByID,
                    TagIds = model.SelectedTagIds
                };

                var response = await _client.PostAsJsonAsync("newsarticles", payload);
                if (response.IsSuccessStatusCode)
                    return (true, "News article created successfully.");

                var err = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                return (false, err.TryGetProperty("message", out var m) ? m.GetString() ?? "Failed" : "Failed");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateNewsArticleAsync(string id, NewsArticleViewModel model)
        {
            try
            {
                var payload = new
                {
                    model.NewsArticleID,
                    model.NewsTitle,
                    model.Headline,
                    model.NewsContent,
                    model.NewsSource,
                    model.CategoryID,
                    model.NewsStatus,
                    model.UpdatedByID,
                    TagIds = model.SelectedTagIds
                };

                var response = await _client.PutAsJsonAsync($"newsarticles/{id}", payload);
                if (response.IsSuccessStatusCode)
                    return (true, "News article updated successfully.");

                var err = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                return (false, err.TryGetProperty("message", out var m) ? m.GetString() ?? "Failed" : "Failed");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> DeleteNewsArticleAsync(string id)
        {
            try
            {
                var response = await _client.DeleteAsync($"newsarticles/{id}");
                var content = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                string msg = content.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                return (response.IsSuccessStatusCode, msg);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // --- CATEGORIES ---
        public async Task<List<CategoryViewModel>> GetCategoriesAsync(string? keyword = null)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(keyword) ? "categories" : $"categories?keyword={Uri.EscapeDataString(keyword)}";
                return await _client.GetFromJsonAsync<List<CategoryViewModel>>(url, _jsonOptions) ?? new List<CategoryViewModel>();
            }
            catch
            {
                return new List<CategoryViewModel>();
            }
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(short id)
        {
            try
            {
                return await _client.GetFromJsonAsync<CategoryViewModel>($"categories/{id}", _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateCategoryAsync(CategoryViewModel model)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("categories", model);
                if (response.IsSuccessStatusCode)
                    return (true, "Category created successfully.");

                var err = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                return (false, err.TryGetProperty("message", out var m) ? m.GetString() ?? "Failed" : "Failed");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateCategoryAsync(short id, CategoryViewModel model)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"categories/{id}", model);
                if (response.IsSuccessStatusCode)
                    return (true, "Category updated successfully.");

                var err = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                return (false, err.TryGetProperty("message", out var m) ? m.GetString() ?? "Failed" : "Failed");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> DeleteCategoryAsync(short id)
        {
            try
            {
                var response = await _client.DeleteAsync($"categories/{id}");
                var content = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                string msg = content.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                return (response.IsSuccessStatusCode, msg);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // --- ACCOUNTS ---
        public async Task<List<AccountViewModel>> GetAccountsAsync(string? keyword = null)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(keyword) ? "accounts" : $"accounts?keyword={Uri.EscapeDataString(keyword)}";
                return await _client.GetFromJsonAsync<List<AccountViewModel>>(url, _jsonOptions) ?? new List<AccountViewModel>();
            }
            catch
            {
                return new List<AccountViewModel>();
            }
        }

        public async Task<AccountViewModel?> GetAccountByIdAsync(short id)
        {
            try
            {
                return await _client.GetFromJsonAsync<AccountViewModel>($"accounts/{id}", _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task<(bool Success, string Message)> CreateAccountAsync(AccountViewModel model)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("accounts", model);
                if (response.IsSuccessStatusCode)
                    return (true, "Account created successfully.");

                var err = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                return (false, err.TryGetProperty("message", out var m) ? m.GetString() ?? "Failed" : "Failed");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateAccountAsync(short id, AccountViewModel model)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"accounts/{id}", model);
                if (response.IsSuccessStatusCode)
                    return (true, "Account updated successfully.");

                var err = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                return (false, err.TryGetProperty("message", out var m) ? m.GetString() ?? "Failed" : "Failed");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> DeleteAccountAsync(short id)
        {
            try
            {
                var response = await _client.DeleteAsync($"accounts/{id}");
                var content = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
                string msg = content.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                return (response.IsSuccessStatusCode, msg);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // --- TAGS ---
        public async Task<List<TagViewModel>> GetTagsAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<TagViewModel>>("tags", _jsonOptions) ?? new List<TagViewModel>();
            }
            catch
            {
                return new List<TagViewModel>();
            }
        }

        // --- REPORTS ---
        public async Task<ReportViewModel?> GetReportAsync(DateTime? start, DateTime? end)
        {
            try
            {
                string query = $"?startDate={start:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}";
                return await _client.GetFromJsonAsync<ReportViewModel>($"reports{query}", _jsonOptions);
            }
            catch
            {
                return null;
            }
        }
    }
}
