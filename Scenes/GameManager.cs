using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
	[Export]
	private Camera3D mainCamera;
	[Export]
	private MeshInstance3D scryArea;
	[Export]
	private PackedScene charControllerScene;
	private CharacterBody3D characterController;
	[Export]
	private Node environment;

	[Export]
	private Vector3 camPos_FullScreen;
	private Vector3 camPos_Default;

	private bool allowInput = true;
	[Export]
	string[] commands;

    public bool AllowInput { get => allowInput; set => allowInput = value; }

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
    }

	private void Setup()
	{
        characterController = (CharacterBody3D)charControllerScene.Instantiate();
		environment.AddChild(characterController);
		scryArea.Visible = false;
        if (mainCamera != null) { camPos_Default = mainCamera.Position; }
        else { camPos_Default = new Vector3(); }
    }

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
				//GD.Print("Running Scry Command");
				//charControl.TriggerInteraction_Scry(true);
				//LinkScryCam();
				break;
		}
	}

	private void LinkScryCam()
	{
		//scryArea.MaterialOverride.Set("albedo_texture", )
	}
}
