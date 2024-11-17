using Godot;
using System;

public partial class CameraManager : Node
{
	private GameManager gameManager;

	[Export]
	private Camera3D mainCamera;
	[Export]
	private Node mainCamDefaultParent;
	private Node characterParent;

	public override void _Ready()
	{
		gameManager = GameManager.Instance;
		characterParent = gameManager.CharacterController;
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("toggle_cameraLock"))
		{
			if (mainCamera.GetParent() == mainCamDefaultParent)
			{
				mainCamera.Reparent(characterParent);
			}
			else
			{
				mainCamera.Reparent(mainCamDefaultParent);
			}
		}
		if (mainCamera.GetParent() == characterParent)
		{
			mainCamera.GlobalRotationDegrees = Vector3.Zero;
		}
	}
}
