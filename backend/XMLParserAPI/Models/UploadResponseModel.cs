using System.Net;

namespace XMLParserAPI.Models
{
    public class UploadResponseModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
