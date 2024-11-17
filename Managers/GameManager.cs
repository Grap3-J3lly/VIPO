using Godot;
using System;

public partial class GameManager : Node
{
    // --------------------------------
    //		    VARIABLES	
    // --------------------------------

	[Export]
	private Camera3D mainCamera;
	[Export]
	private MeshInstance3D scryArea;
	[Export]
	private PackedScene charControllerScene;
	private Node3D characterController;
	[Export]
	private Node environment;

	[Export]
	private Vector3 camPos_FullScreen;
	private Vector3 camPos_Default;

	private bool allowInput = true;
	[Export]
	string[] commands;

	[Export]
	private SubViewport scryCamSubviewport;

	private Camera3D scryCamTemp;

    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------
    public static GameManager Instance { get; private set; }

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

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		if(scryArea.Visible)
		{
			CharacterController charControl = characterController.GetChild<CharacterController>(0);
			scryCamTemp.GlobalPosition = charControl.FootCamSocket.GlobalPosition;
			scryCamTemp.GlobalRotationDegrees = charControl.FootCamSocket.GlobalRotationDegrees;
		}
    }

    // --------------------------------
    //		SETUP LOGIC	
    // --------------------------------

    private void Setup()
	{
        characterController = (Node3D)charControllerScene.Instantiate();
		environment.AddChild(characterController);
		scryArea.Visible = false;
        if (mainCamera != null) { camPos_Default = mainCamera.Position; }
        else { camPos_Default = new Vector3(); }
    }

    // --------------------------------
    //		INTERACTION LOGIC	
    // --------------------------------

    public void ToggleBRB()
	{
		if(mainCamera == null || characterController == null) return;

		characterController.Visible = !characterController.Visible;

		if (mainCamera.Position == camPos_FullScreen) { mainCamera.Position = camPos_Default; }
        else { mainCamera.Position = camPos_FullScreen; }
    }


    public void OnStreamBot_MessageReceived(Variant message)
	{
		CheckForCommands(message.ToString());	
    }

	private void CheckForCommands(string message)
	{
		for (int i = 0; i < commands.Length; i++)
		{
			if (message.Contains(commands[i]))
			{
				RunCommand(i);
			}
		}
	}

	private void RunCommand(int commandId)
	{
		CharacterController charControl = (CharacterController)characterController;
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
				EnableScryCam(true);
				break;
		}
	}

	public void EnableScryCam(bool enableScryCam)
	{
		CharacterController charControl = (CharacterController)characterController;
		scryCamTemp = charControl.FootCam;
		if(enableScryCam)
		{
			scryArea.Visible = true;
			scryCamTemp.Reparent(scryCamSubviewport);

			ViewportTexture texture = scryCamSubviewport.GetTexture();
			texture.ResourceLocalToScene = true;

			scryArea.MaterialOverride.Set("albedo_texture", texture);
		}
		else
		{
			scryCamTemp.Reparent(charControl.FootArea);
			scryArea.Visible = false;
			mainCamera.Current = true;
			scryCamTemp = null;
		}
	}
}
