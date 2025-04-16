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
    public float rotationSpeed = 1.0f;
	[Export]
	public float speed = 5.0f;
	[Export]
	public float jumpVelocity = 4.5f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    // Character Data
    [Export]
    private Color defaultColor;
    [Export]
    private Color speakingColor;
    [Export]
    private StandardMaterial3D characterMaterial;
    [Export]
    private double transitionSpeed = 2;
    [Export]
    private Node3D hatCosmetic;
    [Export]
    private MeshInstance3D kbCosmetic;
    [Export]
    private MeshInstance3D controllerCosmetic;
    [Export]
    private double timerToInputReset = 5;
    [Export]
    private MeshInstance3D lHand;
    [Export]
    private MeshInstance3D rHand;
    [Export]
    private Camera3D footCam;
    [Export]
    private Node3D footArea;
    [Export]
    private Node3D footCamSocket;

    private bool resetting = false;
    private double timer;

    // Interaction Data
    [Export]
    private float interactionTimer = 5;
    [Export]
    private float timerDecrementer = 1;

    [Export]
    private float defaultScale = 1;
    [Export]
    private float enlarge_ScaleAmount = 5;
    [Export]
    private float reduce_ScaleAmount = .2f;

    private bool runIA_ScaleChange = false;
    private bool runIA_Scry = false;

    // Timers
    private float currentTimer_ScaleChange = 0;
    private float currentTimer_Scry = 0;

    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------
    public float Speed { get => speed; }

    public Camera3D FootCam { get => footCam; }
    public Node3D FootArea { get => footArea; set => footArea = value; }
    public Node3D FootCamSocket { get => footCamSocket; }

    public float Enlarge_ScaleAmount { get => enlarge_ScaleAmount; }
    public float Reduce_ScaleAmount { get => reduce_ScaleAmount; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        base._Ready();
        CallDeferred("DelayedAssignManagers");
        timer = timerToInputReset;
        lHand.Visible = false;
        rHand.Visible = true;
        Reset();
        // GD.Print("CharacterController Exists");
    }

    private void DelayedAssignManagers()
    {
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        HandleAudioInput(delta);
        HandleInteractionTimers(delta);
    }

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        InputChecks(delta);
    }

    public override void _Notification(int what)
    {
        base._Notification(what);
        
        if(gameManager == null) { return; }

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

    private void InputChecks(double delta)
    {
        if (gameManager != null && gameManager.AllowInput && gameManager.AllowMovement)
        {
            ToggleInputCosmeticVisibility(0);

            //if (Input.IsActionJustPressed("ui_cancel"))
            //{
            //    GetTree().Quit();
            //}
            //if (Input.IsActionJustPressed("ui_reset"))
            //{
            //    // ResetPosition();
                
            //}
            if (Input.IsActionJustPressed("toggle_hatCosmetic"))
            {
                ToggleHatCosmetic();
            }
            if (Input.IsActionJustPressed("debug_InteractionTrigger"))
            {
                
            }
            HandleMovementInput(delta);
        }
        else
        {
            if (Input.IsAnythingPressed())
            {
                ToggleInputCosmeticVisibility(2);
                timer = timerToInputReset;
            }
            else if (controllerCosmetic.Visible == false || timer <= 0)
            {
                ToggleInputCosmeticVisibility(1);
            }

            if (timer > 0)
            {
                timer -= delta;
            }
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
            velocity.Y = jumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
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

        MainCameraController mainCam = (MainCameraController)gameManager.CameraManager.MainCamera;
        mainCam.MoveCamera(velocity);
    }

    public void Reset()
    {
        Position = resetLocation;
        ((MainCameraController)gameManager.CameraManager.MainCamera).ResetCameraPosition();
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
        lHand.Visible = true;
        rHand.Visible = true;
        switch (inputNumber)
        {
            case 0:
            default: // Turn all off
                kbCosmetic.Visible = false;
                controllerCosmetic.Visible = false;
                lHand.Visible = false;
                rHand.Visible = false;
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
            count += delta * transitionSpeed;
        }

        characterMaterial.AlbedoColor = defaultColor;
        resetting = false;
    }

    // --------------------------------
    //		INTERACTION LOGIC	
    // --------------------------------

    private void HandleInteractionTimers(double delta)
    {
        if(runIA_ScaleChange)
        {
            HandleTimer_ChangeScale(delta);
        }
        if(runIA_Scry)
        {
            HandleTimer_Scry(delta);
        }
    }

    private void HandleTimer_ChangeScale(double delta)
    {
        if (currentTimer_ScaleChange > 0)
        {
            currentTimer_ScaleChange -= ((float)delta * timerDecrementer);
        }
        if (currentTimer_ScaleChange <= 0)
        {
            TriggerInteraction_ChangeScale(defaultScale);
            runIA_ScaleChange = false;
        }
    }

    private void HandleTimer_Scry(double delta)
    {
        if (currentTimer_Scry > 0)
        {
            currentTimer_Scry -= ((float)delta * timerDecrementer);
        }
        if (currentTimer_Scry <= 0)
        {
            TriggerInteraction_Scry(false);
            runIA_Scry = false;
            gameManager.CameraManager.EnableScryCam(false);
        }
    }

    public void TriggerInteraction_ChangeScale(float scaleAmount)
    {
        if(Scale == Vector3.One * enlarge_ScaleAmount && scaleAmount == enlarge_ScaleAmount) { return; }
        runIA_ScaleChange = true;
        currentTimer_ScaleChange = interactionTimer;
        Scale = Vector3.One * scaleAmount;
    }

    public void TriggerInteraction_Scry(bool enable)
    {
        if ((footArea.Visible && enable) || (!footArea.Visible && !enable)) return;
        runIA_Scry = true;
        currentTimer_Scry = interactionTimer;
        footArea.Visible = enable;
    }
}
