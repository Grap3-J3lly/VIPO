using Godot;
using System;

public static class NodeExtensions
{
    // Helper Function Credit to BigThe
    public static T FindFirstChildOfType<T>(this Node parent) where T : Node
    {
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
