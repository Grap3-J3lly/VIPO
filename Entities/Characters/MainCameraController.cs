using Godot;
using System;

public partial class MainCameraController : Camera3D
{
	private GameManager gameManager;
	private Camera3D mainCamera;

	[Export]
	private Vector3 resetPosition = new Vector3(-0.401f, 4.243f, 5.319f);
	private CharacterController characterController;
	[Export]
	private float speedRatio = .00165f;
	private float camSpeed;
	private bool movementActive = false;

	public bool MovementActive { get => movementActive; set => movementActive = value; }

	public override void _Ready()
	{
		gameManager = GameManager.Instance;
		mainCamera = gameManager.CameraManager.MainCamera;
		characterController = gameManager.CharacterController.GetChild<CharacterController>(0);
		camSpeed = speedRatio * characterController.Speed;
	}

	public override void _Process(double delta)
	{
	}

	public void MoveCamera(Vector3 movementVec)
	{
		if(!movementActive) { return; }

		Vector3 currentPosition = mainCamera.Position;
		currentPosition = new Vector3(currentPosition.X + (movementVec.X * camSpeed), currentPosition.Y, currentPosition.Z + (movementVec.Z * camSpeed));
		mainCamera.Position = currentPosition;
	}

	public void ResetCameraPosition()
	{
		mainCamera.Position = resetPosition;
	}
}