using Godot;
using Godot.Collections;
using System;

public partial class CharacterController : CharacterBody3D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;
    private AudioManager audioManager;

    [ExportGroup("Movement Data")]
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

    [ExportGroup("Character Data")]
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
    private Camera3D footCam;
    [Export]
    private Node3D footArea;
    [Export]
    private Node3D footCamSocket;

    private bool resetting = false;

    [ExportGroup("Cosmetics Info")]
    // Cosmetics Info
    [Export]
    private Node3D headArea;
    [Export] 
    private Node3D handArea;
    [Export]
    private Node3D handheldArea;
    [Export]
    private Array<PackedScene> availableHandheldScenes = new Array<PackedScene>();


    [ExportGroup("Interaction Data")]
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
    private float timer_ScaleChange = 0;
    private float timer_Scry = 0;

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
        Setup();
        // GD.Print("CharacterController Exists");

        // Temp
        //string[] voices = DisplayServer.TtsGetVoicesForLanguage("en");
        //voiceId = voices[0];

        EventManager.Instance.ChangeScale += TriggerInteraction_ChangeScale;
        EventManager.Instance.DisplayScryScreen += TriggerInteraction_Scry;
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

    // --------------------------------
    //		SETUP LOGIC	
    // --------------------------------

    private void Setup()
    {
        CallDeferred("DelayedAssignManagers");

        SpawnHandheldCosmetics();

        handArea.Visible = false;
        Reset();
    }

    private void DelayedAssignManagers()
    {
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
    }

    private void SpawnHandheldCosmetics()
    {
        foreach(PackedScene handheld in availableHandheldScenes)
        {
            Node3D newHandheld = handheld.Instantiate<Node3D>();
            handheldArea.AddChild(newHandheld);
            newHandheld.Position = Vector3.Zero;
        }
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

    private void InputChecks(double delta)
    {
        if (gameManager != null && gameManager.AllowInput && gameManager.AllowMovement)
        {
            if (Input.IsActionJustPressed("toggle_hatCosmetic"))
            {
                ToggleHatCosmetic();
            }
            if (Input.IsActionJustPressed("debug_InteractionTrigger"))
            {
                // DisplayServer.TtsSpeak("This is a test message, wow!", voiceId);
            }

            // GD.Print($"CharacterController.cs: Allowed Input? {gameManager.AllowInput} Allowed Movement? {gameManager.AllowMovement}");
            HandleMovementInput(delta);
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

        // GD.Print($"CharacterController.cs: Velocity: {Velocity}");

        MainCameraController mainCam = (MainCameraController)gameManager.CameraManager.MainCamera;
        mainCam.MoveCamera(velocity);
    }

    public void Reset()
    {
        Position = resetLocation;
        if(gameManager != null && gameManager.CameraManager != null)
        {
            ((MainCameraController)gameManager.CameraManager.MainCamera).ResetCameraPosition();
        }
    }

    private void ToggleHatCosmetic()
    {
        headArea.Visible = !headArea.Visible;
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
        if (timer_ScaleChange > 0)
        {
            timer_ScaleChange -= ((float)delta * timerDecrementer);
        }
        if (timer_ScaleChange <= 0)
        {
            ChangeScale(defaultScale);
            runIA_ScaleChange = false;
        }
    }

    private void HandleTimer_Scry(double delta)
    {
        if (timer_Scry > 0)
        {
            timer_Scry -= ((float)delta * timerDecrementer);
        }
        if (timer_Scry <= 0)
        {
            TriggerInteraction_Scry(false);
            runIA_Scry = false;
            gameManager.CameraManager.EnableScryCam(false);
        }
    }

    public void TriggerInteraction_ChangeScale(bool isIncreasing)
    {
        float scaleAmount = isIncreasing? enlarge_ScaleAmount : reduce_ScaleAmount;
        ChangeScale(scaleAmount);
    }
    private void ChangeScale(float scaleAmount)
    {
        if(Scale == Vector3.One * enlarge_ScaleAmount && scaleAmount == enlarge_ScaleAmount) { return; }
        runIA_ScaleChange = true;
        timer_ScaleChange = interactionTimer;
        Scale = Vector3.One * scaleAmount;
    }


    public void TriggerInteraction_Scry(bool enable)
    {
        GD.Print($"CharacterController.cs: Triggering Interaction: Scry");
        if ((footArea.Visible && enable) || (!footArea.Visible && !enable)) return;
        runIA_Scry = true;
        timer_Scry = interactionTimer;
        footArea.Visible = enable;
    }
}
