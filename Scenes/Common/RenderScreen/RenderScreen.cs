using Godot;
using System;

public partial class RenderScreen : Node3D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;

	[Export]
	private MeshInstance3D liveMesh;

	// Camera Texture Vars
	[Export]
	private string targetFeed = "OBS Virtual Camera";

	private CameraTexture texture;

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;

		CameraServer.MonitoringFeeds = true;
		CallDeferred("Setup");
	}

    // --------------------------------
    //			SETUP LOGIC	
    // --------------------------------

    private void Setup()
	{
		CameraTextureSetup();
	}

	private void CameraTextureSetup()
	{
    	// CameraServer.MonitoringFeeds = true;
		GD.Print($"RenderScreen.cs: Camera Server Feed Count: {CameraServer.GetFeedCount()}");
		

		for (int i = 0; i < CameraServer.GetFeedCount(); i++)
		{
			CameraFeed feed;
			feed = CameraServer.GetFeed(i);
			GD.Print($"RenderScreen.cs: Camera Feed Name: {feed.GetName()}");
			GD.Print($"RenderScreen.cs: Camera Feed Formats: {feed.Formats}");

			if (feed.GetName() == targetFeed)
			{
				var format = (Godot.Collections.Dictionary)feed.Formats[0];
				feed.SetFormat(0, format);

				feed.FeedIsActive = true;

				texture = new CameraTexture();
				texture.CameraFeedId = i + 1;//Arrays start at 1 now I guess...
				// Material mat = liveMesh.GetSurfaceOverrideMaterial(0);
				Material mat = (Material)liveMesh.Mesh.Get("material");
				mat.Set("albedo_texture", texture);
				mat.Set("shading_mode", 0);

				int width = (int)format["width"];
				int height = (int)format["height"];
				GD.Print($"RenderScreen.cs: Camera Width / Height: {(float)width / (float)height}");
			}
		}
	}

}
