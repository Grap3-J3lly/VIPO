using Godot;
using System;
using System.Text.Json.Nodes;

public static class JsonNodeExtensions
{
    public const char pathSeparator = '/';

    public static JsonNode GetJsonNodeValueByString(this JsonNode node, string valuePath)
    {
        string[] nodePaths = valuePath.Split(pathSeparator);

        for (int i = 0; i < nodePaths.Length; i++)
        {
            if (node == null) break;
            if(int.TryParse(nodePaths[i], out int nodeArrayIndex))
            {
                node = node[nodeArrayIndex];
            }
            else
            {
                node = node[nodePaths[i]];
            }
            // GD.Print($"JsonNodeExtensions.cs: {node?.ToString()}");
        }
        return node;
    }
}