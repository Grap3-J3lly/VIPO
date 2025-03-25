using Godot;
using System;

public partial class CameraManager : Node
{
    // --------------------------------
    //		    VARIABLES	
    // --------------------------------
    private GameManager gameManager;

	[Export]
	private Camera3D mainCamera;
    private Camera3D scryCamTemp;
    [Export]
    private SubViewport scryCamSubviewport;

    [Export]
	private Node mainCamDefaultParent;
	private Node characterParent;

    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------

	public Camera3D MainCamera { get => mainCamera; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
        CallDeferred("DelayedAssignment");
	}

    private void DelayedAssignment()
    {
        gameManager = GameManager.Instance;
        characterParent = gameManager.CharacterController;
    }

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("toggle_cameraLock"))
		{
			if (mainCamera.GetParent() == mainCamDefaultParent)
			{
                mainCamera.Reparent(characterParent);
                ((MainCameraController)mainCamera).MovementActive = true;
            }
			else
			{
				mainCamera.Reparent(mainCamDefaultParent);
                ((MainCameraController)mainCamera).MovementActive = false;
            }
		}
		if (mainCamera.GetParent() == characterParent)
		{
			mainCamera.RotationDegrees = Vector3.Zero;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (gameManager.ScryArea.Visible)
        {
            CharacterController charControl = characterParent.GetChild<CharacterController>(0);
            scryCamTemp.GlobalPosition = charControl.FootCamSocket.GlobalPosition;
            scryCamTemp.GlobalRotationDegrees = charControl.FootCamSocket.GlobalRotationDegrees;
        }
    }

    public void EnableScryCam(bool enableScryCam)
    {
        CharacterController charControl = characterParent.GetChild<CharacterController>(0);
        scryCamTemp = charControl.FootCam;
        if (enableScryCam)
        {
            gameManager.ScryArea.Visible = true;
            scryCamTemp.Reparent(scryCamSubviewport);

            ViewportTexture texture = scryCamSubviewport.GetTexture();
            texture.ResourceLocalToScene = true;

            gameManager.ScryArea.MaterialOverride.Set("albedo_texture", texture);
        }
        else
        {
            scryCamTemp.Reparent(charControl.FootArea);
            gameManager.ScryArea.Visible = false;
            mainCamera.Current = true;
            scryCamTemp = null;
        }
    }
}
