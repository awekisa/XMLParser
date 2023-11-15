using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using XMLParserAPI.Controllers;
using XMLParserAPI.Models;

namespace XMLParserAPITests
{
    public class Tests
    {
        private string BASE_PATH = Directory.GetCurrentDirectory();
        private Mock<IFormFile> _mockFile;
        private DocumentController _sut;

        [SetUp]
        public void Setup()
        {
            _mockFile = new Mock<IFormFile>();
        }

        [Test]
        public async Task WhenUploadValidXmlThenJsonIsSaved()
        {
            // Arrange
            _mockFile.Setup(m => m.OpenReadStream()).Returns(new MemoryStream(File.ReadAllBytes($"{BASE_PATH}\\TestValidXMLFiles\\sample.xml")));
            _sut = new DocumentController();

            // Act
            var response = await _sut.UploadAsync(new UploadRequestModel { File = _mockFile.Object, FileName = "result" });

            // Assert
            var fileLocation = $"{BASE_PATH}\\OutputFiles\\result.json";
            Assert.IsTrue(File.Exists(fileLocation));

            // cleanup
            File.Delete(fileLocation);
        }

        [Test]
        public async Task WhenUploadMultipleValidXmlFilesThenJsonFilesAreSaved()
        {
            // Arrange
            _sut = new DocumentController();

            // Act
            var files = Directory.EnumerateFiles($"{BASE_PATH}\\TestValidXMLFiles\\", "*.xml").ToArray();
            var tasks = files
                .AsParallel()
                .Select((f) =>
                {
                    var mockFile = new Mock<IFormFile>();
                    mockFile.Setup(m => m.OpenReadStream()).Returns(new MemoryStream(File.ReadAllBytes(f)));
                    var fileName = ExtractFileNameFromLocation(f, $"{BASE_PATH}\\TestValidXMLFiles\\", ".xml");
                    return _sut.UploadAsync(new UploadRequestModel { File = mockFile.Object, FileName = fileName });
                })
                .ToList();

            await Task.WhenAll(tasks);

            // Assert
            Assert.That(tasks.Count(), Is.EqualTo(files.Length));

            foreach (var task in tasks)
            {
                Assert.That(task.Result, Is.TypeOf<UploadResponseModel>());
                Assert.That(task.Result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                var fileLocation = task?.Result?.Message?.Replace("File saved at ", "");
                Assert.IsTrue(File.Exists(fileLocation));
            }

            // cleanup
            Thread.Sleep(2000); // so we have time to manually check that the output files are created, before cleanup 
            var outputFiles = Directory.EnumerateFiles($"{BASE_PATH}\\OutputFiles\\", "*.json").ToArray();
            foreach(var outputFile in outputFiles)
            {
                File.Delete(outputFile);
            }
        }

        [Test]
        public async Task WhenUploadMultipleFilesThenOnlyValidXmlsAreSaved()
        {
            // Arrange
            _sut = new DocumentController();

            // Act
            var files = Directory.EnumerateFiles($"{BASE_PATH}\\TestValidXMLFiles\\", "*.xml")
                .Concat(Directory.EnumerateFiles($"{BASE_PATH}\\TestInvalidXMLFiles\\", "*.*"))
                .ToArray();

            var tasks = files
                .AsParallel()
                .Select((f) =>
                {
                    var mockFile = new Mock<IFormFile>();
                    mockFile.Setup(m => m.OpenReadStream()).Returns(new MemoryStream(File.ReadAllBytes(f)));
                    var fileName = ExtractFileNameFromLocation(f, $"{BASE_PATH}\\TestValidXMLFiles\\", ".xml");
                    return _sut.UploadAsync(new UploadRequestModel { File = mockFile.Object, FileName = fileName });
                })
                .ToList();

            await Task.WhenAll(tasks);

            // Assert
            Assert.That(tasks.Count(), Is.EqualTo(files.Length));

            var successfullTasks = 0;
            var unsuccessfullTasks = 0;

            foreach (var task in tasks)
            {
                Assert.That(task.Result, Is.TypeOf<UploadResponseModel>());

                if (task.Result.StatusCode == HttpStatusCode.OK)
                {
                    var fileLocation = task?.Result?.Message?.Replace("File saved at ", "");
                    Assert.IsTrue(File.Exists(fileLocation));
                    successfullTasks++;
                }
                else if (task.Result.StatusCode == HttpStatusCode.InternalServerError) {
                    unsuccessfullTasks++;
                }
            }

            Assert.That(successfullTasks, Is.EqualTo(5)); // number of files in TestValidXMLFiles folder
            Assert.That(unsuccessfullTasks, Is.EqualTo(4)); // number of files in TestInvalidXMLFiles folder

            // cleanup
            Thread.Sleep(2000); // so we have time to manually check that the output files are created, before cleanup 
            var outputFiles = Directory.EnumerateFiles($"{BASE_PATH}\\OutputFiles\\", "*.json").ToArray();
            foreach (var outputFile in outputFiles)
            {
                File.Delete(outputFile);
            }
        }

        [Test]
        public async Task WhenUploadTxtFileThenErrorResponse()
        {
            // Arrange
            _mockFile.Setup(m => m.OpenReadStream()).Returns(new MemoryStream(File.ReadAllBytes($"{BASE_PATH}\\TestInvalidXMLFiles\\textFile.txt")));
            _sut = new DocumentController();

            // Act
            var response = await _sut.UploadAsync(new UploadRequestModel { File = _mockFile.Object, FileName = "result" });

            // Assert
            Assert.That(response.StatusCode == HttpStatusCode.InternalServerError);
            Assert.IsNull(response.Message);
            Assert.NotNull(response.ErrorMessage);
            Assert.That(response.ErrorMessage.StartsWith("Data at the root level is invalid"));
        }

        [Test]
        public async Task WhenUploadJsonFileThenErrorResponse()
        {
            // Arrange
            _mockFile.Setup(m => m.OpenReadStream()).Returns(new MemoryStream(File.ReadAllBytes($"{BASE_PATH}\\TestInvalidXMLFiles\\jsonFile.json")));
            _sut = new DocumentController();

            // Act
            var response = await _sut.UploadAsync(new UploadRequestModel { File = _mockFile.Object, FileName = "result" });

            // Assert
            Assert.That(response.StatusCode == HttpStatusCode.InternalServerError);
            Assert.IsNull(response.Message);
            Assert.NotNull(response.ErrorMessage);
            Assert.That(response.ErrorMessage.StartsWith("Data at the root level is invalid"));
        }

        [Test]
        public async Task WhenUploadInvalidXMLFileThenErrorResponse()
        {
            // Arrange
            _mockFile.Setup(m => m.OpenReadStream()).Returns(new MemoryStream(File.ReadAllBytes($"{BASE_PATH}\\TestInvalidXMLFiles\\invalidXML.xml")));
            _sut = new DocumentController();

            // Act
            var response = await _sut.UploadAsync(new UploadRequestModel { File = _mockFile.Object, FileName = "result" });

            // Assert
            Assert.That(response.StatusCode == HttpStatusCode.InternalServerError);
            Assert.IsNull(response.Message);
            Assert.NotNull(response.ErrorMessage);
            Assert.That(response.ErrorMessage.StartsWith("Unexpected end of file has occurred"));
        }

        [Test]
        public async Task WhenUploadEmptyXmlFileThenErrorResponse()
        {
            // Arrange
            _mockFile.Setup(m => m.OpenReadStream()).Returns(new MemoryStream(File.ReadAllBytes($"{BASE_PATH}\\TestInvalidXMLFiles\\empty.xml")));
            _sut = new DocumentController();

            // Act
            var response = await _sut.UploadAsync(new UploadRequestModel { File = _mockFile.Object, FileName = "result" });

            // Assert
            Assert.That(response.StatusCode == HttpStatusCode.InternalServerError);
            Assert.IsNull(response.Message);
            Assert.NotNull(response.ErrorMessage);
            Assert.That(response.ErrorMessage.StartsWith("Root element is missing"));
        }

        private string ExtractFileNameFromLocation(string text, string findText1, string findText2)
        {
            int indexBeginngingWord1 = text.IndexOf(findText1);//Find the beginning index of the word1
            int indexEndOfWord1 = indexBeginngingWord1 + findText1.Length;//Add the length of the word1 to starting index to find the end of the word1
            int indexBeginningWord2 = text.LastIndexOf(findText2);//Find the beginning index of word2

            if (indexBeginngingWord1 < 0 || indexEndOfWord1 < 0)
            {
                return string.Empty;
            }
            int length = indexBeginningWord2 - indexEndOfWord1;//Length of the substring by subtracting index beginning of word2 from the end of word1
            string substring = text.Substring(indexEndOfWord1, length);//Get the substring
            return substring;
        }
    }
}