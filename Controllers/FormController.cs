using api2webapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace api2webapp.Controllers
{
    [Route("DynamicForm")]
    public class FormController : Controller
    {
        private static List<ApiEndpoint> _endpoints;

        public IActionResult Index(string filePath)
        {
            _endpoints = Services.YamlParser.EndpointExtractor(filePath);
            return View("Index", _endpoints); // List of links to individual forms
        }

        [HttpGet("{formName}")]
        public IActionResult Form(string formName)
        {
            if (_endpoints == null)
            {
                // Re-load YAML if needed
                string yamlPath = Path.Combine(Directory.GetCurrentDirectory(), "api-docs/user123/api.yaml");
                _endpoints = Services.YamlParser.EndpointExtractor(yamlPath);
            }

            var endpoint = _endpoints.FirstOrDefault(e => GetSlugFromPath(e.Path).Equals(formName, StringComparison.OrdinalIgnoreCase));

            if (endpoint == null)
                return NotFound("Form not found");

            return View("Form", endpoint);
        }

        private string GetSlugFromPath(string path)
        {
            return path.Trim('/').Split('/').Last().Replace("{", "").Replace("}", "");
        }

        [HttpPost("SubmitForm")]
        public async Task<IActionResult> SubmitForm(string path, [FromForm] Dictionary<string, string> formData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5000");

                var jsonData = System.Text.Json.JsonSerializer.Serialize(formData);
                var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(path, content);

                if (response.IsSuccessStatusCode)
                {
                    return View("Success");
                }
                else
                {
                    return View("Error");
                }
            }
        }
    }
}
