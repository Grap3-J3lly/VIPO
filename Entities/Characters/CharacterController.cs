using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
    private AudioManager audioManager;

    // Movement Data
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

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
    private Tween colorTween;


    public override void _Ready()
    {
        base._Ready();
        audioManager = AudioManager.Instance;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleAudioInput(delta);
    }

    public override void _PhysicsProcess(double delta)
	{
        
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }
        HandleMovementInput(delta);
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

    private void HandleAudioInput(double delta)
    {
        if (audioManager.IsCapturingAudio())
        {
            characterMaterial.AlbedoColor = speakingColor;
            return;
        }
        characterMaterial.AlbedoColor = defaultColor;
        //ShiftToDefaultColor();
    }

    private async void ShiftToDefaultColor()
    {
        if(colorTween != null)
        {
            return;
        }

        colorTween = GetTree().CreateTween();
        colorTween.TweenProperty(characterMaterial, "albedo_color", defaultColor, speed);
        //colorTween.Finished += ClearTween;
        //colorTween.Connect("finished", ClearTween());
        await ToSignal(colorTween, Tween.SignalName.Finished);
        ClearTween();
    }

    private void ClearTween()
    {
        colorTween.Kill();
        // return new Godot.Callable();
    }
}
