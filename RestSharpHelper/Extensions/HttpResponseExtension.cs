using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharpHelper.Extensions.Interfaces;
using RestSharpHelper.Models;
using System.Reflection;
using System.Text;

namespace RestSharpHelper.Extensions
{
    public class HttpResponseExtension : IHttpResponseExtension
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HttpResponseExtension> _logger;
        private readonly RestSharpConfigurationModel _restSharpConfiguration;
        public HttpResponseExtension(
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpResponseExtension> logger,
        IOptions<RestSharpConfigurationModel> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _restSharpConfiguration = options.Value;
        }
        #region Create REST Request
        /// <summary>
        /// Get Token
        /// </summary>
        private string? Token
            => _httpContextAccessor.HttpContext.Session.GetString(_restSharpConfiguration.TokenSession ?? "_token");
        /// <summary>
        /// Get Token Scheme
        /// </summary>
        private string? TokenScheme
            => $"{_restSharpConfiguration.TokenSession ?? "Bearer"} ";
        private RestClient RESTClient => new(_restSharpConfiguration.HostUrl ?? string.Empty);
        /// <summary>
        /// Get Byte Form FiLe
        /// </summary>
        /// <param name="formFiLe"x/param>
        /// <returns></returns>
        private static async Task<byte[]> GetBytes(IFormFile formFile)
        {
            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        /// <summary>
        /// Convert Body to Body Paraws
        /// </sumwary>
        /// < pa raw nawe="body"x/paraw>
        /// <returns></returns>
        private static BodyFormModel? ToBodyParams(object body)
        {
            BodyFormModel? bodyForms = new();

            if (body == null) return null;
            bodyForms.BodyForms = body.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(s => s.PropertyType != typeof(IFormFile))
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(body, null));
            bodyForms.FileForms = body.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(s => s.PropertyType == typeof(IFormFile))
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(body, null) as IFormFile);

            return bodyForms;
        }
        /// <summary>
        /// Create REST Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private RestRequest CreateRestRequest(string url, List<QueryParamModel>? query)
        {
            RestRequest request = new(url);
            #region Add Header
            if (!string.IsNullOrEmpty(Token))
            {
                request.AddHeader("Authorization", $"{TokenScheme} {Token}");
            }
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            #endregion
            #region Add Query Param
            if (query != null)
            {
                foreach (var item in query)
                {
                    if (!string.IsNullOrEmpty(item.Key))
                        request.AddQueryParameter(item.Key ?? string.Empty, item.Value);
                }
            }
            #endregion
            return request;
        }
        /// <summary>
        /// Create Json Rest Request
        /// </summary>
        /// <param name="url"></param>/param>
        /// <param name="data"></param>
        /// <param name="query"></param>/param>
        /// <returns></returns>
        private RestRequest CreateJsonRestRequest(string url, object? data, List<QueryParamModel>? query)
        {
            var request = CreateRestRequest(url, query);
            if (data != null) request.AddJsonBody(data);
            return request;
        }
        /// < summary>
        /// Create Form Rest Request
        /// </summary>
        /// <param name="urL"X/param>
        /// <param name="data"X/param>
        /// < pa ram name="query"X/param>
        /// <returns></returns>/returns>
        private async Task<RestRequest> CreateFormRestRequest(string url, object? data, List<QueryParamModel>? query)
        {
            var request = CreateRestRequest(url, query);
            if (data == null) return request;
            var bodypams = ToBodyParams(data);
            if (bodypams == null) return request;
            // Add normaL Param
            if (bodypams.BodyForms != null)
            {
                foreach (var param in bodypams.BodyForms)
                {
                    request.AddParameter(Parameter.CreateParameter(param.Key, param.Value, ParameterType.GetOrPost));
                }
            }
            // Add FiLw Param
            if (bodypams.FileForms != null)
            {
                foreach (var file in bodypams.FileForms)
                {
                    if (file.Value != null)
                    {
                        request.AddFile(file.Key, await GetBytes(file.Value), file.Value.FileName);
                    }
                }
            }
            return request;
        }
        #endregion
        #region GET Request
        /// <summary>
        /// Create Get Response Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<T?> CreateGetResponseAsync<T>(string url, RestRequest request)
        {
            try
            {
                var responseContent = await RESTClient.GetAsync<string>(request);

                if (string.IsNullOrEmpty(responseContent))
                    return default;

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseContent);
                return response;
            }
            catch (Exception ex)
            {
                string message = $"Get {url} Error: {ex.Message}";
                _logger?.LogError(message);
            }

            return default;
        }
        /// <summary>
        /// Get Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> GetResponseAsync<T>(string url, List<QueryParamModel>? query)
        {
            var request = CreateRestRequest(url, query);
            var response = await CreateGetResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Get Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> GetJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = CreateJsonRestRequest(url, data, query);
            var response = await CreateGetResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Get Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> GetFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = await CreateFormRestRequest(url, data, query);
            var response = await CreateGetResponseAsync<T>(url, request);
            return response;
        }
        #endregion
        #region Post Request
        /// <summary>
        /// Create Post Response Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<T?> CreatePostResponseAsync<T>(string url, RestRequest request)
        {
            try
            {
                var responseContent = await RESTClient.PostAsync<string>(request);

                if (string.IsNullOrEmpty(responseContent))
                    return default;

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseContent);
                return response;
            }
            catch (Exception ex)
            {
                string message = $"Post {url} Error: {ex.Message}";
                _logger?.LogError(message);
            }

            return default;
        }
        /// <summary>
        /// Post Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> PostResponseAsync<T>(string url, List<QueryParamModel>? query)
        {
            var request = CreateRestRequest(url, query);
            var response = await CreatePostResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Post Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> PostJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = CreateJsonRestRequest(url, data, query);
            var response = await CreatePostResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Post Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> PostFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = await CreateFormRestRequest(url, data, query);
            var response = await CreatePostResponseAsync<T>(url, request);
            return response;
        }
        #endregion
        #region Put Request
        /// <summary>
        /// Create Put Response Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<T?> CreatePutResponseAsync<T>(string url, RestRequest request)
        {
            try
            {
                var responseContent = await RESTClient.PutAsync<string>(request);

                if (string.IsNullOrEmpty(responseContent))
                    return default;

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseContent);
                return response;
            }
            catch (Exception ex)
            {
                string message = $"Put {url} Error: {ex.Message}";
                _logger?.LogError(message);
            }

            return default;
        }
        /// <summary>
        /// Put Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> PutResponseAsync<T>(string url, List<QueryParamModel>? query)
        {
            var request = CreateRestRequest(url, query);
            var response = await CreatePutResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Put Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> PutJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = CreateJsonRestRequest(url, data, query);
            var response = await CreatePutResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Put Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> PutFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = await CreateFormRestRequest(url, data, query);
            var response = await CreatePutResponseAsync<T>(url, request);
            return response;
        }
        #endregion
        #region Delete Request
        /// <summary>
        /// Create Delete Response Async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<T?> CreateDeleteResponseAsync<T>(string url, RestRequest request)
        {
            try
            {
                var responseContent = await RESTClient.DeleteAsync<string>(request);

                if (string.IsNullOrEmpty(responseContent))
                    return default;

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseContent);
                return response;
            }
            catch (Exception ex)
            {
                string message = $"Put {url} Error: {ex.Message}";
                _logger?.LogError(message);
            }

            return default;
        }
        /// <summary>
        /// Delete Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> DeleteResponseAsync<T>(string url, List<QueryParamModel>? query)
        {
            var request = CreateRestRequest(url, query);
            var response = await CreateDeleteResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Delete Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> DeleteJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = CreateJsonRestRequest(url, data, query);
            var response = await CreateDeleteResponseAsync<T>(url, request);
            return response;
        }
        /// <summary>
        /// Delete Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public async Task<T?> DeleteFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query)
        {
            var request = await CreateFormRestRequest(url, data, query);
            var response = await CreateDeleteResponseAsync<T>(url, request);
            return response;
        }
        #endregion
    }
}
