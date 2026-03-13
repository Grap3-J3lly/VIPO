using Godot;
using System;

public partial class MainCameraController : Camera3D
{
	private GameManager gameManager;
	private Camera3D mainCamera;

	[Export]
	private Vector3 resetPosition = new Vector3(-0.401f, 4.243f, 5.319f);
	private float targetSpeed = 0;
	[Export]
	private float speedRatio = .00165f;
	private float camSpeed;
	private bool trackingObj = false;
	private bool movementActive = true;

	public void SET_MovementActive(bool canMove)
	{ movementActive = canMove; }
	public bool GET_MovementActive()
	{ return movementActive; }

	public bool TrackingObj { get => trackingObj; set => trackingObj = value; }

	public override void _Ready()
	{
		gameManager = GameManager.Instance;
		mainCamera = gameManager.CameraManager.MainCamera;

		EventManager.Instance.SpeedChange += SetSpeedValues;
		EventManager.Instance.EnableMovement += SET_MovementActive;

		GD.Print($"MainCameraController.cs: TrackingOBJ: {trackingObj}, Movement Active? {movementActive}");
	}

	public override void _Process(double delta)
	{
	}
	public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
		MoveCamera();
	}

	private void SetSpeedValues(float newSpeed)
	{
		targetSpeed = newSpeed;
		camSpeed = speedRatio * targetSpeed;
		GD.Print($"MainCameraController.cs: Target Speed vs. Cam Speed: {targetSpeed} and {camSpeed}");
	}

	public void MoveCamera()
	{
		if(!trackingObj || !movementActive) { return; }

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		Vector3 currentPosition = mainCamera.Position;
		Vector3 movementVec = new Vector3(inputDir.X * targetSpeed, 0, inputDir.Y * targetSpeed);
		currentPosition = new Vector3(currentPosition.X + (movementVec.X * camSpeed), currentPosition.Y, currentPosition.Z + (movementVec.Z * camSpeed));
		mainCamera.Position = currentPosition;
	}

	public void ResetCameraPosition()
	{
		mainCamera.Position = resetPosition;
	}
}