using RestSharpHelper.Models;

namespace RestSharpHelper.Extensions.Interfaces
{
    /// <summary>
    /// Http Response Extension Interface
    /// </summary>
    public interface IHttpResponseExtension
    {
        #region GET Request
        /// <summary>
        /// Get Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> GetResponseAsync<T>(string url, List<QueryParamModel>? query);
        /// <summary>
        /// Get Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> GetJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        /// <summary>
        /// Get Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> GetFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        #endregion
        #region POST Request
        /// <summary>
        /// Post Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> PostResponseAsync<T>(string url, List<QueryParamModel>? query);
        /// <summary>
        /// Post Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> PostJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        // <summary>
        /// Post Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> PostFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        #endregion
        #region PUT Request
        /// <summary>
        /// Put Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> PutResponseAsync<T>(string url, List<QueryParamModel>? query);
        /// <summary>
        /// Put Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> PutJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        /// <summary>
        /// Put Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> PutFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        #endregion
        #region DELETE Request
        /// <summary>
        /// Delete Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> DeleteResponseAsync<T>(string url, List<QueryParamModel>? query);
        /// <summary>
        /// Delete Json Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> DeleteJsonResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        /// <summary>
        /// Delete Form Response
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="url">url</param>
        /// <param name="data">data</param>
        /// <param name="query">query</param>
        /// <returns>data</returns>
        public Task<T?> DeleteFormResponseAsync<T>(string url, object? data, List<QueryParamModel>? query);
        #endregion
    }
}
