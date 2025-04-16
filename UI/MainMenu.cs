using Godot;
using System;

public partial class MainMenu : Control
{
    private GameManager gameManager;
    [Export]
    private TextureButton resetButton;
    [Export]
    private TextureButton quitButton;
    // private CharacterController characterController;

    public override void _Ready()
    {
        // Visible = false;
        resetButton.Pressed += OnReset;
        quitButton.Pressed += OnQuit;

        gameManager = GameManager.Instance;

        CallDeferred("DelayedSetup");
    }

    private void DelayedSetup()
    {
        resetButton.GrabFocus();
        GD.Print("Reset Button Has Focus? " + resetButton.HasFocus());
    }

    public void OnReset()
    {
        gameManager.Reset();
    }

    public void OnQuit()
    {
        GetTree().Quit();
    }
}
