using Godot;
using System;

public partial class MainCameraController : Camera3D
{
	// --------------------------------
    //      	VARIABLES
    // --------------------------------

	[Export]
	private Vector3 resetPosition = new Vector3(-0.401f, 4.243f, 5.319f);
	private float targetSpeed = 0;
	[Export]
	private float speedRatio = .00165f;
	private float camSpeed;
	private bool trackingObj = false;
	private bool movementActive = true;

	[Export]
	private Node mainCamDefaultParent;
	private Node characterParent;

	// --------------------------------
    //      	PROPERTIES
    // --------------------------------

	public void SET_MovementActive(bool canMove)
	{ movementActive = canMove; }
	public bool GET_MovementActive()
	{ return movementActive; }

	public bool TrackingObj { get => trackingObj; set => trackingObj = value; }

	// --------------------------------
    //      STANDARD LOGIC	
    // --------------------------------

	public override void _Ready()
	{
		EventManager.Instance.SpeedChange += SetSpeedValues;
		EventManager.Instance.EnableMovement += SET_MovementActive;
		EventManager.Instance.Reset += ToggleCameraLock;
		EventManager.Instance.Reset += ResetCameraPosition;

		characterParent = ObjectPool.Instance.CharacterController.GetParent();

		GD.Print($"MainCameraController.cs: TrackingOBJ: {trackingObj}, Movement Active? {movementActive}");
	}

	public override void _Process(double delta)
	{
		if(movementActive && Input.IsActionJustPressed("toggle_cameraLock"))
		{
			ToggleCameraLock(resetToDefault: false);
		}
		if (GetParent() == characterParent)
		{
			RotationDegrees = Vector3.Zero;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
		MoveCamera();
	}


	// --------------------------------
	//		CAMERA MOVEMENT LOGIC	
    // --------------------------------

	private void SetSpeedValues(float newSpeed)
	{
		targetSpeed = newSpeed;
		camSpeed = speedRatio * targetSpeed;
		GD.Print($"MainCameraController.cs: Target Speed vs. Cam Speed: {targetSpeed} and {camSpeed}");
	}

	public void MoveCamera()
	{
		if(!trackingObj || !movementActive) 
		{ 
			// GD.Print($"MainCameraController.cs: Tracking OBJ? {trackingObj} MovementActive? {movementActive}");
			return; 
		}

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		Vector3 currentPosition = Position;
		Vector3 movementVec = new Vector3(inputDir.X * targetSpeed, 0, inputDir.Y * targetSpeed);
		currentPosition = new Vector3(currentPosition.X + (movementVec.X * camSpeed), currentPosition.Y, currentPosition.Z + (movementVec.Z * camSpeed));
		Position = currentPosition;
		// GD.Print($"MainCameraController.cs: Input Direction: {inputDir} New Position: {Position}, Target Speed: {targetSpeed}, Cam Speed: {camSpeed}");
	}

	public void ResetCameraPosition(bool value)
	{
		Position = resetPosition;
	}

	public void ToggleCameraLock(bool resetToDefault)
    {
        if (GetParent() == mainCamDefaultParent && !resetToDefault)
        {
            Reparent(characterParent);
            TrackingObj = true;
        }
        else
        {
            Reparent(mainCamDefaultParent);
            TrackingObj = false;
        }

        GD.Print($"MainCameraController.cs: Movement Active? {TrackingObj}");
    }
}