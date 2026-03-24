using Godot;
using System;

public static class NodeExtensions
{
    // Helper Function Credit to BigThe
    public static T FindFirstChildOfType<T>(this Node parent) where T : Node
    {
        GD.Print($"NodeExtension.cs: Parent: {parent.Name}");
        foreach (var child in parent.GetChildren())
        {
            if (child.GetType().IsAssignableTo(typeof(T)))
            {
                return (T)child;
            }
        }
        return null;
    }
}
