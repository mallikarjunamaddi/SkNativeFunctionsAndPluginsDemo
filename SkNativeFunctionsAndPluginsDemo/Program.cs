using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using SkNativeFunctionsAndPluginsDemo.Plugins.CareerAdvisor;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// Get Azure OpenAI configuration from appsettings
var deploymentName = configuration["AzureOpenAI:DeploymentName"];
var endpoint = configuration["AzureOpenAI:Endpoint"];

var builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(
    deploymentName: deploymentName ?? throw new InvalidOperationException("DeploymentName not found in configuration"),
    endpoint: endpoint ?? throw new InvalidOperationException("Endpoint not found in configuration"),
    credentials: new DefaultAzureCredential()
);

var kernel = builder.Build();

//To create and register the custom plugin to the Kernel from class type.
kernel.ImportPluginFromType<CareerAdvisorPlugin>();
kernel.CreatePluginFromType<CareerAdvisorPlugin>();

Console.WriteLine("To Add new job title: ");

Console.WriteLine("Enter Job Title: ");
var title = Console.ReadLine();

Console.WriteLine("Enter Company name");
var company = Console.ReadLine();

Console.WriteLine("Enter rank of the Job Title");
var rank = Console.ReadLine();


var result = await kernel.InvokeAsync(
    pluginName: "CareerAdvisorPlugin",
    functionName: "AddNewJobTitle",
    arguments: new ()
    {
        {"title", title},
        {"company", company},
        {"rank", rank}
    }
);
Console.WriteLine(result);

var careerList = await kernel.InvokeAsync(
        pluginName: "CareerAdvisorPlugin",
        functionName: "GetCareerHistory"
);


Console.WriteLine("------- Your Current Career History ------");
Console.WriteLine(careerList);