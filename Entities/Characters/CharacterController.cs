using Godot;
using System;

public partial class CharacterController : CharacterBody3D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;
    private AudioManager audioManager;

    // Movement Data
    [Export]
    private Vector3 resetLocation = new Vector3(0, 0, 0);
    [Export]
    public float RotationSpeed = 1.0f;
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;
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
    [Export]
    private MeshInstance3D hatCosmetic;
    [Export]
    private MeshInstance3D kbCosmetic;
    [Export]
    private MeshInstance3D controllerCosmetic;
    [Export]
    private double timerToInputReset = 5;
    private double timer;

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        base._Ready();
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
        timer = timerToInputReset;
        ResetPosition();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleAudioInput(delta);
    }

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        if (gameManager.AllowInput)
        {
            ToggleInputCosmeticVisibility(0);
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                GetTree().Quit();
            }
            if (Input.IsActionJustPressed("ui_reset"))
            {
                ResetPosition();
            }
            if (Input.IsActionJustPressed("toggle_hatCosmetic"))
            {
                ToggleHatCosmetic();
            }
            HandleMovementInput(delta);
        }
        else
        {
            if(Input.IsAnythingPressed())
            {
                ToggleInputCosmeticVisibility(2);
                timer = timerToInputReset;
            }
            else if (controllerCosmetic.Visible == false || timer <= 0)
            {
                ToggleInputCosmeticVisibility(1);
            }

            if(timer > 0)
            {
                timer -= delta;
            }
        }
    }

    public override void _Notification(int what)
    {
        base._Notification(what);
        
        if(what == MainLoop.NotificationApplicationFocusIn)
        {
            gameManager.AllowInput = true;
        }
        if(what == MainLoop.NotificationApplicationFocusOut)
        {
            gameManager.AllowInput = false; 
        }
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

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
        // Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (inputDir != Vector2.Zero)
        {
            velocity.X = inputDir.X * Speed;
            velocity.Z = inputDir.Y * Speed;

            LookAt(Position - new Vector3(inputDir.X, 0, inputDir.Y));
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

    private void ToggleHatCosmetic()
    {
        hatCosmetic.Visible = !hatCosmetic.Visible;
    }

    /// <summary>
    /// When movement is not allowed, display input source cosmetic in front of the character
    /// 0 for nothing, 1 for KB&M, 2 for Controller
    /// </summary>
    private void ToggleInputCosmeticVisibility(int inputNumber = 0)
    {
        switch (inputNumber)
        {
            case 0:
            default: // Turn all off
                kbCosmetic.Visible = false;
                controllerCosmetic.Visible = false;
                break;
            case 1: // KB&M Only
                kbCosmetic.Visible = true;
                controllerCosmetic.Visible = false;
                break;
            case 2: // Controller Only
                kbCosmetic.Visible = false;
                controllerCosmetic.Visible = true;
                break;
        }
    }

    // --------------------------------
    //			AUDIO LOGIC 	
    // --------------------------------

    private void HandleAudioInput(double delta)
    {
        if (audioManager.IsCapturingAudio())
        {
            characterMaterial.AlbedoColor = speakingColor;
            ShiftToDefaultColor(delta);
            return;
        }
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
