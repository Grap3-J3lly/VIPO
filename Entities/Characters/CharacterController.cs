using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
    private AudioManager audioManager;

    // Movement Data
    [Export]
    private Vector3 resetLocation = new Vector3(0, 0, 0);
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private bool allowInput = true;

    // Character Data
    [Export]
    private Color defaultColor;
    [Export]
    private Color speakingColor;
    [Export]
    private StandardMaterial3D characterMaterial;
    [Export]
    private double speed = 2;
    private bool resetting = false;


    public override void _Ready()
    {
        base._Ready();
        audioManager = AudioManager.Instance;
        ResetPosition();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleAudioInput(delta);
    }

    public override void _PhysicsProcess(double delta)
	{
        if(allowInput)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                GetTree().Quit();
            }
            if (Input.IsActionJustPressed("ui_reset"))
            {
                ResetPosition();
            }
            HandleMovementInput(delta);
        }
	}

    public override void _Notification(int what)
    {
        base._Notification(what);
        
        if(what == MainLoop.NotificationApplicationFocusIn)
        {
            allowInput = true;
        }
        if(what == MainLoop.NotificationApplicationFocusOut)
        {
            allowInput = false; 
        }
    }

    private void HandleMovementInput(double delta)
	{
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            velocity.Y = JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void ResetPosition()
    {
        Position = resetLocation;
    }

    private void HandleAudioInput(double delta)
    {
        if (audioManager.IsCapturingAudio())
        {
            characterMaterial.AlbedoColor = speakingColor;
            return;
        }
        ShiftToDefaultColor(delta);
    }

    private async void ShiftToDefaultColor(double delta)
    {
        if (resetting)
        {
            return;
        }
        resetting = true;

        double count = 0;

        while(count < 1)
        {
            characterMaterial.AlbedoColor = speakingColor.Lerp(defaultColor, (float)count);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            count += delta * speed;
        }

        characterMaterial.AlbedoColor = defaultColor;
        resetting = false;
    }

   
}
