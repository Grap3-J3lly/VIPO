using Godot;
using System;

public partial class UIManager : Node
{
    public static UIManager Instance { get; private set; }
    private GameManager gameManager;

    [Export]
    private Control uiBase;

    [Export]
    private PackedScene mainMenuBaseScene;
    private Control mainMenuBase;

    public override void _Ready()
    {
        Instance = this;
        gameManager = GameManager.Instance;
    }

    public void ToggleMenu()
    {
        if(mainMenuBase != null)
        {
            mainMenuBase.QueueFree();
            mainMenuBase = null;
            gameManager.AllowMovement = true;
        }
        else
        {
            mainMenuBase = (Control)mainMenuBaseScene.Instantiate();
            uiBase.AddChild(mainMenuBase);
            gameManager.AllowMovement = false;
        }
    }
}
