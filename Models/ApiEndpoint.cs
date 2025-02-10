namespace api2webapp.Models
{
    public class ApiEndpoint
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
