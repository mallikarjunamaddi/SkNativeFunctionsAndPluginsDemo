using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SkNativeFunctionsAndPluginsDemo.Plugins.CareerAdvisor
{
    internal class CareerAdvisorPlugin
    {
        [KernelFunction, Description("Get the list of jobs worked during user's career")]
        public static string GetCareerHistory()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var content = File.ReadAllText($"{currentDirectory}/Data/recentJobs.txt");
            return content; 
        }

        [KernelFunction, Description("Add a new job to the user's career list")]
        public static string AddNewJobTitle(
                [Description("User's Job Title")] string title,
                [Description("Company Name")] string company,
                [Description("Rank of the job title")] string rank
            )
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = $"{currentDirectory}/Data/recentJobs.txt";
            var content = File.ReadAllText(filePath);
            var careerHistory = (JsonArray) JsonNode.Parse(content);

            var job = new JsonObject
            {
                ["title"] = title,
                ["company"] = company,
                ["rank"] = rank
            };

            careerHistory.Insert(0, job);
            File.WriteAllText(filePath, JsonSerializer.Serialize(
                    careerHistory, new JsonSerializerOptions { WriteIndented = true }
                )
            );
            
            return $"Added new job title - {title}";
        }
    }
}
