using Godot;
using System;

public partial class GameManager : Node
{
	// --------------------------------
	//		    VARIABLES	
	// --------------------------------

	[Export]
	private CameraManager cameraManager;
	private UIManager uiManager;
	[Export]
	private MeshInstance3D scryArea;
	[Export]
	private PackedScene charControllerScene;
	private Node3D characterController;
	[Export]
	private Node environment;
	[Export]
	private ObjectPool objectPool;

	[Export]
	private Vector3 camPos_FullScreen;
	private Vector3 camPos_Default;

	// TODO: Implement Application States (Menu State vs. Character State)

	// Allow Input is for any and all input, including menu navigation
	private bool allowInput = true;
	// Allow Movement is for ignoring character movement inputs, generally for during menu navigation
	private bool allowMovement = true;
	[Export]
	string[] commands;

	[Signal]
	public delegate void ToggleTwitchEventHandler(bool isActive);
    [Signal]
    public delegate void UpdateChatTextEventHandler(string newChat);
    [Signal]
    public delegate void UpdateChatTextureEventHandler(Texture2D newTexture);
	[Signal]
	public delegate void ImageReceivedEventHandler(Image newImage);


    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------
    public static GameManager Instance { get; private set; }
	public CameraManager CameraManager { get => cameraManager; set => cameraManager = value; }
	public MeshInstance3D ScryArea { get => scryArea; }

	public Node3D CharacterController { get => characterController; }
    public bool AllowInput { get => allowInput; set => allowInput = value; }
	public bool AllowMovement { get => allowMovement; set => allowMovement = value; }

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

    // --------------------------------
    //		SETUP LOGIC	
    // --------------------------------

	private void DelayedSetup()
	{
        uiManager = UIManager.Instance;
    }

    private void Setup()
	{
        characterController = (Node3D)charControllerScene.Instantiate();
		objectPool.AddChild(characterController);
		objectPool.objects.Add(characterController);
		
		scryArea.Visible = false;
        if (cameraManager.MainCamera != null) { camPos_Default = cameraManager.MainCamera.Position; }
        else { camPos_Default = new Vector3(); }

		objectPool.CallDeferred("TrySpawnFamiliar", "Gandalf");
    }

    // --------------------------------
    //		GENERAL LOGIC	
    // --------------------------------

	public void Reset()
	{
        CharacterController charController = CharacterController.FindFirstChildOfType<CharacterController>();
        charController.Reset();
        cameraManager.ToggleCameraLock(true);
        uiManager.ToggleMenu();
    }

    // --------------------------------
    //		INTERACTION LOGIC	
    // --------------------------------

    public void InputChecks()
	{
        if (AllowInput && Input.IsActionJustPressed("ui_reset"))
		{
			uiManager.ToggleMenu();
		}

    }

    public void ToggleBRB()
	{
		if(cameraManager.MainCamera == null || characterController == null) return;

		characterController.Visible = !characterController.Visible;

		if (cameraManager.MainCamera.Position == camPos_FullScreen) { cameraManager.MainCamera.Position = camPos_Default; }
        else { cameraManager.MainCamera.Position = camPos_FullScreen; }
    }


    public void OnStreamBot_MessageReceived(Variant message)
	{
		CheckForInteractions(message.ToString());	
    }

	private void CheckForInteractions(string message)
	{
		// GD.Print(message);
		for (int i = 0; i < commands.Length; i++)
		{
			if (message.Contains("\"type\":\"Action\"") && message.Contains(commands[i]))
			{
				string userName = "";
				// Check for User, grab string between next two quotation marks
				if(message.Contains("\"user\":"))
				{
					int userIndex = message.Find("\"user\":\"") + 8; // 8 being the size of the search criteria we want to pass
					int nameLastIndex = message.Find("\"", userIndex);
					userName = message.Substring(userIndex, nameLastIndex - userIndex);

					// GD.Print("User Index: " + userIndex);
					// GD.Print("Name Last Index: " + nameLastIndex);
                }
				RunCommand(i, userName);
				break;
			}
		}
	}

	private void RunCommand(int commandId, string userName)
	{
		CharacterController charControl = characterController.GetChild<CharacterController>(0);
		switch (commandId)
		{
			case 0:
                GD.Print("Running Enlarge Command");
                charControl.TriggerInteraction_ChangeScale(charControl.Enlarge_ScaleAmount);
                break;
			case 1:
				GD.Print("Running Reduce Command");
				charControl.TriggerInteraction_ChangeScale(charControl.Reduce_ScaleAmount);
				break;
			case 2:
				GD.Print("Running Scry Command");
				charControl.TriggerInteraction_Scry(true);
				cameraManager.EnableScryCam(true);
				break;
			case 3:
				GD.Print("Running Find Familiar Command");
				objectPool.TrySpawnFamiliar(userName);
				break;

		}
	}

	
}
