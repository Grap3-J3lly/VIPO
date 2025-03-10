using Godot;
using System;

public partial class Familiar : CharacterBody3D
{
    [Export]
	public float speed = 5.0f;
	// public float jumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _PhysicsProcess(double delta)
	{
        
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

    private void Move(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        //if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        //    velocity.Y = jumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        // Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (inputDir != Vector2.Zero)
        {
            velocity.X = inputDir.X * speed;
            velocity.Z = inputDir.Y * speed;

            LookAt(Position - new Vector3(inputDir.X, 0, inputDir.Y));
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
