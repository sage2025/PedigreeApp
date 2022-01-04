using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

using System.Web;
using System.Reflection;
using System.Text;
using Svg;
using System.Drawing.Imaging;

namespace Pedigree.App.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;

        public FilesController(ILogger<FilesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id:int}/forms/{formId:int}")]
        [ProducesResponseType(typeof(FileFormSubmissionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<FileFormSubmissionResult> ViewForm(int id, int formId)
        {
            _logger.LogInformation($"viewing the form#{formId} for File ID={id}");
            await Task.Delay(1000);
            return new FileFormSubmissionResult { FormId = formId, FileId = id };
        }

        [HttpPost("{id:int}/forms")]
        [ProducesResponseType(typeof(FileFormSubmissionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<ActionResult<FileFormSubmissionResult>> SubmitForm(int id, [FromForm] FileForm form)
        {
            _logger.LogInformation($"Validating the form#{form.FormId} for File ID={id}");

            if (form.Courses == null || form.Courses.Length == 0)
            {
                return BadRequest("Please enter at least one course.");
            }

            if (form.FileFile == null || form.FileFile.Length < 1)
            {
                return BadRequest("The uploaded file is empty.");
            }

            if (Path.GetExtension(form.FileFile.FileName) != ".pdf")
            {
                return BadRequest($"The uploaded file {form.FileFile.Name} is not a PDF file.");
            }

            var filePath = Path.Combine(@"App_Data", $"{DateTime.Now:yyyyMMddHHmmss}.pdf");
            new FileInfo(filePath).Directory?.Create();
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                _logger.LogInformation($"Saving file [{form.FileFile.FileName}]");
                await form.FileFile.CopyToAsync(stream);
                _logger.LogInformation($"\t The uploaded file is saved as [{filePath}].");
            }

            var result = new FileFormSubmissionResult { FormId = form.FormId, FileId = id, FileSize = form.FileFile.Length };
            return CreatedAtAction(nameof(ViewForm), new { id, form.FormId }, result);
        }

        /// <summary>
        /// An Example API Endpoint Accepting Multiple Files
        /// </summary>
        /// <param name="id"></param>
        /// <param name="certificates"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/certificates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<ActionResult<List<CertificateSubmissionResult>>> SubmitCertificates(int id, [Required] List<IFormFile> certificates)
        {
            var result = new List<CertificateSubmissionResult>();

            if (certificates == null || certificates.Count == 0)
            {
                return BadRequest("No file is uploaded.");
            }

            foreach (var certificate in certificates)
            {
                var filePath = Path.Combine(@"App_Data", id.ToString(), @"Certificates", certificate.FileName);
                new FileInfo(filePath).Directory?.Create();

                await using var stream = new FileStream(filePath, FileMode.Create);
                await certificate.CopyToAsync(stream);
                _logger.LogInformation($"The uploaded file [{certificate.FileName}] is saved as [{filePath}].");

                result.Add(new CertificateSubmissionResult { FileName = certificate.FileName, FileSize = certificate.Length });
            }

            return Ok(result);
        }

        [HttpGet("uploadSvg/{svgData}")]
        public async Task<string> uploadSvg(string svgData)
        {
            /*
            var byteArray = Convert.FromBase64String(svgData);

            //var byteArray = Encoding.ASCII.GetBytes(svgData);
            using (var stream = new MemoryStream(byteArray))
            {
                SvgDocument svgDocument = SvgDocument.Open<SvgDocument>(stream);
                var bitmap = svgDocument.Draw();
                bitmap.Save("1.png", ImageFormat.Png);
            }*/
            SvgDocument svgDocument = SvgDocument.Open("data:image/svg+xml;base64," + svgData);
            var bitmap = svgDocument.Draw();
            bitmap.Save("1.png", ImageFormat.Png);

            return ("success");
        }


        [HttpGet("DownloadFiles/{id:int}")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            // validation and get the file

            var filePath = $"{id}.txt";
            if (!System.IO.File.Exists(filePath))
            {
                await System.IO.File.WriteAllTextAsync(filePath, "Hello World!");
            }
            
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }



            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, contentType, Path.GetFileName(filePath));
            
        }

        [HttpGet("download/{documentid:int}")]
        public async Task<string> Download(int documentid)
        {
            string filename = $"{documentid}.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory()+ "\\wwwroot\\Ind_report", filename);
            Console.WriteLine(filePath);
            if (!System.IO.File.Exists(filePath))
            
                return null;
           
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return Convert.ToBase64String(bytes);
            //return File(bytes,GetContentType(filePath),filename);
        }

        private string GetContentTypeDown(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".bmp", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }



    public class FileForm
    {
        [Required] public int FormId { get; set; }
        [Required] public string[] Courses { get; set; }
        [Required] public IFormFile FileFile { get; set; }
    }

    public class FileFormSubmissionResult
    {
        public int FileId { get; set; }
        public int FormId { get; set; }
        public long FileSize { get; set; }
    }

    public class CertificateSubmissionResult
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
    }
}
