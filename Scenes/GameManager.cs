using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
	[Export]
	private Camera3D mainCamera;
	[Export]
	private PackedScene charControllerScene;
	private CharacterBody3D characterController;
	[Export]
	private Node environment;

	[Export]
	private Vector3 camPos_FullScreen;
	private Vector3 camPos_Default;

	public override void _Ready()
	{
		base._Ready();
        Instance = this;
        Setup();
	}

	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		if (Input.IsActionJustPressed("toggle_screenBRB"))
		{
			ToggleBRB();
		}
    }

	private void Setup()
	{
        characterController = (CharacterBody3D)charControllerScene.Instantiate();
		environment.AddChild(characterController);

        if (mainCamera != null) { camPos_Default = mainCamera.Position; }
        else { camPos_Default = new Vector3(); }
    }

    public void ToggleBRB()
	{
		if(mainCamera == null || characterController == null) return;

		characterController.Visible = !characterController.Visible;

		if (mainCamera.Position == camPos_FullScreen) { mainCamera.Position = camPos_Default; }
        else { mainCamera.Position = camPos_FullScreen; }
    }
}
