using System.Text.Json.Serialization;

namespace AzureDevopsTooling.Models;

public class Variable
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("isSecret")]
    public bool IsSecret { get; set; }
}

public class ProjectReference
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class VariableGroupProjectReference
{
    [JsonPropertyName("projectReference")]
    public ProjectReference ProjectReference { get; set; }
}

public class VariableGroup
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "Vsts";

    [JsonPropertyName("variables")]
    public Dictionary<string, Variable> Variables { get; set; }

    [JsonPropertyName("variableGroupProjectReferences")]
    public List<VariableGroupProjectReference> VariableGroupProjectReferences { get; set; }
}

