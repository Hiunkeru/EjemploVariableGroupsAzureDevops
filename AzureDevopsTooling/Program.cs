using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AzureDevopsTooling.Models;

namespace AzureDevopsTooling;

class Program
{
    static async Task Main(string[] args)
    {
        string organization = "[Tu organización]";
        string project = "[Tu proyecto]";
        string token = "[Token]";

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{token}")));

        // Obtener el ID del proyecto
        var projectInfo = await GetProjectInfoAsync(httpClient, organization, project);

        if (projectInfo == null)
        {
            Console.WriteLine("Error: No se pudo obtener el ID del proyecto.");
            return;
        }

        var variableGroup = new VariableGroup
        {
            Name = "NombreDeTuVariableGroup",
            Description = "Descripción del grupo de variables",
            Variables = new Dictionary<string, Variable>
                {
                    { "variable1", new Variable { Value = "valor1", IsSecret = false } },
                    { "variable2", new Variable { Value = "valor2", IsSecret = true } }
                },
            VariableGroupProjectReferences = new List<VariableGroupProjectReference>
                {
                    new VariableGroupProjectReference
                    {
                        ProjectReference = new ProjectReference { Id = projectInfo.Id, Name = projectInfo.Name }
                    }
                }
        };

        var requestUrl = $"https://dev.azure.com/{organization}/{project}/_apis/distributedtask/variablegroups?api-version=7.1-preview.1";


        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonString = JsonSerializer.Serialize(variableGroup, jsonOptions);
        Console.WriteLine("JSON enviado: " + jsonString);

        var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(requestUrl, content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
            Console.WriteLine("Variable group created successfully:");
            Console.WriteLine(result);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error creating variable group:");
            Console.WriteLine(error);
        }
    }

    static async Task<ProjectReference> GetProjectInfoAsync(HttpClient httpClient, string organization, string projectName)
    {
        var requestUrl = $"https://dev.azure.com/{organization}/_apis/projects?api-version=7.1";
        var response = await httpClient.GetAsync(requestUrl);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
            if (result != null)
            {
                var projects = result.RootElement.GetProperty("value").EnumerateArray();
                foreach (var project in projects)
                {
                    if (project.GetProperty("name").GetString() == projectName)
                    {
                        return new ProjectReference
                        {
                            Id = project.GetProperty("id").GetString(),
                            Name = project.GetProperty("name").GetString()
                        };
                    }
                }
            }
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error retrieving project ID:");
            Console.WriteLine(error);
        }

        return null;
    }
}

