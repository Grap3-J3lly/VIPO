using Godot;
using System;

public partial class GameManager : Node
{
	// --------------------------------
	//		    VARIABLES	
	// --------------------------------

	private MenuManager menuManager;
	[Export]
	private PackedScene charControllerScene;
	private Node3D characterController;
	[Export]
	private Node environment;
	[Export]
	private ObjectPool objectPool;

	[Export]
	private int defaultScreenIndex = 1;

	[Export]
	private Vector3 camPos_FullScreen;
	private Vector3 camPos_Default;

	// TODO: Implement Application States (Menu State vs. Character State)

	// Allow Input is for any and all input, including menu navigation
	private bool allowInput = true;
	// Allow Movement is for ignoring character movement inputs, generally for during menu navigation
	private bool allowMovement = true;
	[Export]
	private string[] commands;

	[Signal]
	public delegate void ToggleTwitchEventHandler(bool isActive);
    [Signal]
    public delegate void ChatReceivedEventHandler(string newChat);
    [Signal]
    public delegate void UpdateChatTextureEventHandler(Texture2D newTexture);
	[Signal]
	public delegate void ImageReceivedEventHandler(Image newImage);


    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------
    public static GameManager Instance { get; private set; }
	public PackedScene CharControllerScene { get => charControllerScene; }
    public bool AllowInput 
	{ 
		get => allowInput;
		set 
		{ 
			allowInput = value;
			EventManager.Instance.EnableInputEventEmit(allowInput);
		}
	}
	public bool AllowMovement 
	{ 
		get => allowMovement;
		set 
		{
			allowMovement = value; 
			EventManager.Instance.EnableMovementEventEmit(allowMovement);
		}
	}

    // --------------------------------
    //		STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		base._Ready();
        Instance = this;
        Setup();
		CallDeferred("DelayedSetup");
	}

	public override void _Process(double delta)
	{
		InputChecks();
    }

	public override void _Notification(int what)
    {
        base._Notification(what);

        if(what == MainLoop.NotificationApplicationFocusIn)
        {
			GD.Print($"GameManager.cs: Enabling Input");
            AllowInput = true;
        }
        if(what == MainLoop.NotificationApplicationFocusOut)
        {
			GD.Print($"GameManager.cs: Disabling Input");
            AllowInput = false; 
        }
    }

    // --------------------------------
    //		SETUP LOGIC	
    // --------------------------------

	private void DelayedSetup()
	{
        menuManager = MenuManager.Instance;
    }

    private void Setup()
	{
		GD.Print($"GameManager.cs: Intiating Setup");
		DisplayServer.WindowSetCurrentScreen(defaultScreenIndex);

		objectPool.CallDeferred("TrySpawnFamiliar", "Gandalf");
    }

    // --------------------------------
    //		INTERACTION LOGIC	
    // --------------------------------

    public void InputChecks()
	{
        if (AllowInput && Input.IsActionJustPressed("ui_reset"))
		{
			menuManager.ToggleMenu();
		}

    }	
}
