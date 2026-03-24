using Godot;
using System;

public partial class MainMenu : Control
{
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

        CallDeferred("DelayedSetup");
    }

    private void DelayedSetup()
    {
        resetButton.GrabFocus();
        GD.Print("Reset Button Has Focus? " + resetButton.HasFocus());
    }

    public void OnReset()
    {
        EventManager.Instance.ResetEventEmit(true);
        MenuManager.Instance.ToggleMenu();
    }

    public void OnQuit()
    {
        GD.Print($"MainMenu.cs: Quitting VIPO");
        GetTree().Quit();
    }
}
