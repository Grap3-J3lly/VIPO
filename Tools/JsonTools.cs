using Godot;
using System;
using System.Text.Json.Nodes;

public static class JsonTools
{
    public static JsonNode ParseJson(string messageToParse, string dataPath)
    {
        JsonNode root = JsonNode.Parse(messageToParse);

        if (root == null)
        {
            GD.Print($"ToggleChatInputButton.cs: Failed to Parse");
            return null;
        }

        JsonNode result = root.GetJsonNodeValueByString(dataPath);

        return result;
    }
}