using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace api2webapp.Controllers
{
    public class YamlController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}
        IConfiguration _iconfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public YamlController(IConfiguration iconfiguration, IWebHostEnvironment hostingEnvironment)
        {
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult YamlUpload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile yaml)
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            if (yaml == null || yaml.Length==0)
            {
                return await Task.FromResult<IActionResult>(BadRequest("No file uploaded"));
            }
            else
            {
                string userId = "user123";
                var yamlPath = Path.Combine(contentRootPath, "api-docs", userId);

                if (!Directory.Exists(yamlPath))
                {
                    Directory.CreateDirectory(yamlPath);
                }
                var filePath = Path.Combine(yamlPath, yaml.FileName);
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                       await yaml.CopyToAsync(fs);
                    }
                }
                catch (Exception ex)
                {
                    // Log or inspect the error
                    Console.WriteLine($"Error: {ex.Message}");
                    return await Task.FromResult<IActionResult>(StatusCode(500, "Internal Server Error"));
                }
                var obj = new FormController();
                var result = obj.Index(filePath);
                
                return RedirectToAction("Index", "Form", new { filePath });
            }
        }
    }
}
