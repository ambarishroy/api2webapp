using api2webapp.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace api2webapp.Services
{
    public class YamlParser
    {
        public static List<ApiEndpoint> EndpointExtractor(string yamlPath)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            var yamlText = File.ReadAllText(yamlPath);
            var yamlData = deserializer.Deserialize<Dictionary<string, object>>(yamlText);

            var endpoints = new List<ApiEndpoint>();

            if (yamlData.ContainsKey("paths"))
            {
                var paths = (Dictionary<object, object>)yamlData["paths"];

                foreach (var path in paths)
                {
                    var methods = (Dictionary<object, object>)path.Value;

                    foreach (var method in methods)
                    {
                        if (!method.Key.ToString().Equals("post", StringComparison.OrdinalIgnoreCase))
                            continue; // Process only POST requests

                        var endpoint = new ApiEndpoint
                        {
                            Path = path.Key.ToString(),
                            Method = method.Key.ToString().ToUpper(),
                            Parameters = new Dictionary<string, string>()
                        };

                        if (method.Value is Dictionary<object, object> methodData &&
                            methodData.ContainsKey("requestBody"))
                        {
                            var requestBody = (Dictionary<object, object>)methodData["requestBody"];
                            var content = (Dictionary<object, object>)requestBody["content"];
                            var schema = (Dictionary<object, object>)((Dictionary<object, object>)content["application/json"])["schema"];
                            var properties = (Dictionary<object, object>)schema["properties"];

                            foreach (var param in properties)
                            {
                                if (param.Value is Dictionary<object, object> paramValue &&
                                    paramValue.ContainsKey("type") &&
                                    paramValue["type"] is string type)
                                {
                                    endpoint.Parameters[param.Key.ToString()] = type;
                                }
                            }
                        }

                        endpoints.Add(endpoint);
                    }
                }
            }

            return endpoints;
        }
    }
}
