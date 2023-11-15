using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml;
using XMLParserAPI.Models;

namespace XMLParserAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : Controller
    {
        private readonly IConfiguration _configuration;
        private string BASE_PATH = Directory.GetCurrentDirectory();

        public DocumentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<UploadResponseModel> UploadAsync([FromForm] UploadRequestModel request)
        {
            try
            {
                var fileNameWithPath = string.Empty;

                using (var reader = new StreamReader(request.File.OpenReadStream()))
                {
                    var xmlDocument = await ReadXmlFileInput(reader);

                    var json = ConvertXmlToJson(xmlDocument);

                    fileNameWithPath = await SaveJsonFile(json, request.FileName);
                }

                return new UploadResponseModel { StatusCode = System.Net.HttpStatusCode.OK, Message = $"File saved at {fileNameWithPath}" };
            }
            catch (Exception ex)
            {
                return new UploadResponseModel { StatusCode = System.Net.HttpStatusCode.InternalServerError, ErrorMessage = ex.Message };
            }
        }

        private async Task<XmlDocument> ReadXmlFileInput(StreamReader reader)
        {
            var fileContent = await reader.ReadToEndAsync();
            XmlDocument document = new XmlDocument();
            document.LoadXml(fileContent);
            return document;
        }

        private string ConvertXmlToJson(XmlDocument document)
        {
            return JsonConvert.SerializeXmlNode(document, Newtonsoft.Json.Formatting.None);
        }

        private async Task<string> SaveJsonFile(string json, string fileName)
        {
            var targetFolder = Path.Combine(BASE_PATH, "OutputFiles");

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var fileNameWithPath = Path.Combine(targetFolder, $"{fileName}.json");
            await System.IO.File.WriteAllTextAsync(fileNameWithPath, json);

            return fileNameWithPath;
        }
    }
}
