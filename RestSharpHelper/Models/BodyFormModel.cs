using Microsoft.AspNetCore.Http;

namespace RestSharpHelper.Models
{
    /// <summary>
    /// Body Form Model
    /// </summary>
    public class BodyFormModel
    {
        /// <summary>
        /// Body Forms
        /// </summary>
        public IDictionary<string, object?>? BodyForms { get; set; }
        /// <summary>
        /// File Forms
        /// </summary>
        public IDictionary<string, IFormFile?>? FileForms { get; set; }

    }
}
