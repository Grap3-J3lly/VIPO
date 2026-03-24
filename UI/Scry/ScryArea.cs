using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class ScryArea : MeshInstance3D
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
	private CharacterController charControl;
	private bool isCamActive = false;
	private Node3D cameraSocket;

	private Dictionary<Camera3D, Node3D> scryAreas = new Dictionary<Camera3D, Node3D>();

    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------

	public Camera3D MainCamera { get => mainCamera; }

	// --------------------------------
	//	    STANDARD FUNCTIONS	
    // --------------------------------

	public override void _Ready()
	{
		Visible = false;
		EventManager.Instance.DisplayScryScreen += EnableScryCam;
		charControl = ObjectPool.Instance.CharacterController;
		EventManager.Instance.PopulateScryAreaDataEventEmit(scryAreas);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
        ScryCamTracking();
	}

	public void EnableScryCam(bool enableScryCam)
    {
        scryCamTemp = scryAreas.Keys.First();
		cameraSocket = scryAreas.Values.First();
		isCamActive = enableScryCam;
        if (isCamActive)
        {
            scryCamTemp.Reparent(scryCamSubviewport);

            ViewportTexture texture = scryCamSubviewport.GetTexture();
            texture.ResourceLocalToScene = true;

            MaterialOverride.Set("albedo_texture", texture);
			Visible = true;
        }
        else
        {
			Visible = false;
            scryCamTemp.Reparent(cameraSocket);
            mainCamera.Current = true;
            scryCamTemp = null;
			cameraSocket = null;
        }
    }

	private void ScryCamTracking()
	{
		if (isCamActive && scryCamTemp != null && cameraSocket != null)
        {
            scryCamTemp.GlobalPosition = cameraSocket.GlobalPosition;
            scryCamTemp.GlobalRotationDegrees = cameraSocket.GlobalRotationDegrees;
        }
	}
}
