using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using WordParserLibrary;

namespace WordParserApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger)
    {
        _logger = logger;
    }

    [HttpPost("generatexml")]
    public IActionResult GenerateXml(IFormFile file, [FromQuery] bool generateGuid = false)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
            {
                var legalAct = new LegalAct(wordDoc);
                var xml = legalAct.XmlGenerator.GenerateString(generateGuid);
                return Content(xml, "application/xml");
            }
        }
    }

    [HttpGet("generatexml")]
    public IActionResult GetGenerateXml()
    {
        var version = "1.0.0.22";

        var htmlForm = $@"
            <html>
            <body>
            <h1>Generowanie XML z pliku DOCX v{version}</h1>
            <form action='/api/generatexml' method='post' enctype='multipart/form-data'>
            <label for='file'>Plik DOCX:</label>
            <input type='file' id='file' name='file' accept='.docx'>
            <input type='submit' value='Wygeneruj XML'>
            </form>
            </body>
            </html>";
        return Content(htmlForm, "text/html");
    }

    [HttpPost("validate")]
    public IActionResult Validate(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
            {
                var legalAct = new LegalAct(wordDoc);
                var validatedStream = legalAct.GetStream(new List<string> { "VALIDATE" });
                validatedStream.Position = 0;
                return File(validatedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "VALIDATED_" + file.FileName);
            }
        }
    }

    [HttpPost("markHyperlinks")]
    public IActionResult MarkHyperlinks(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
            {
                var legalAct = new LegalAct(wordDoc);
                var validatedStream = legalAct.GetStream(new List<string> { "HYPERLINKS" });
                validatedStream.Position = 0;
                return File(validatedStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "HYPERLINKS_" + file.FileName);
            }
        }
    }
    
    [HttpPost("generateAmendmentsTable")]
    public IActionResult GenerateAmendmentsTable(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            stream.Position = 0;

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
            {
                var legalAct = new LegalAct(wordDoc);
                var validatedStream = legalAct.XlsxGenerator.GenerateXlsx();
                validatedStream.Position = 0;
                return File(validatedStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AMENDMENTS_TABLE_" + Path.ChangeExtension(file.FileName, "xlsx"));
            }
        }
    }
}