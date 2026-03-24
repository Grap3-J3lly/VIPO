using Godot;
using System;

public partial class MenuManager : Node
{
    public static MenuManager Instance { get; private set; }

    [Export]
    private Control menuBase;

    [Export]
    private PackedScene mainMenuBaseScene;
    private Control mainMenuBase;

    public override void _Ready()
    {
        Instance = this;
    }

    public void ToggleMenu()
    {
        if(mainMenuBase != null)
        {
            mainMenuBase.QueueFree();
            mainMenuBase = null;
            EventManager.Instance.EnableMovementEventEmit(enable: true);
            GD.Print($"MenuManager.cs: Enabling Movement");
        }
        else
        {
            mainMenuBase = (Control)mainMenuBaseScene.Instantiate();
            menuBase.AddChild(mainMenuBase);
            EventManager.Instance.EnableMovementEventEmit(enable: false);
            GD.Print($"MenuManager.cs: Disabling Movement");
        }
    }
}
