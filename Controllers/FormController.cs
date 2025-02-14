using Microsoft.AspNetCore.Mvc;

namespace api2webapp.Controllers
{
    [Route("DynamicForm")]
    public class FormController : Controller
    {
        public IActionResult Index(string filePath)
        {
            var endpoints = Services.YamlParser.EndpointExtractor(filePath);
            return View(endpoints);
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
                    return View("Success"); // Redirect to success page
                }
                else
                {
                    return View("Error"); // Redirect to error page
                }
            }
        }


    }
}
