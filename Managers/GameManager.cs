using Godot;
using System;

public partial class GameManager : Node
{
	// --------------------------------
	//		    VARIABLES	
	// --------------------------------

	[Export]
	private CameraManager cameraManager;

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

	private bool allowInput = true;
	[Export]
	string[] commands;


    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------
    public static GameManager Instance { get; private set; }
	public CameraManager CameraManager { get => cameraManager; set => cameraManager = value; }
	public MeshInstance3D ScryArea { get => scryArea; }

	public Node3D CharacterController { get => characterController; }
    public bool AllowInput { get => allowInput; set => allowInput = value; }

    // --------------------------------
    //		STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		base._Ready();
        Instance = this;
        Setup();
	}

	public override void _Process(double delta)
	{

	}

    // --------------------------------
    //		SETUP LOGIC	
    // --------------------------------

    private void Setup()
	{
        characterController = (Node3D)charControllerScene.Instantiate();
		objectPool.AddChild(characterController);
		objectPool.objects.Add(characterController);
		
		scryArea.Visible = false;
        if (cameraManager.MainCamera != null) { camPos_Default = cameraManager.MainCamera.Position; }
        else { camPos_Default = new Vector3(); }

    }

    // --------------------------------
    //		INTERACTION LOGIC	
    // --------------------------------

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
