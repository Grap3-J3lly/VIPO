using Godot;
using System;

public partial class RenderScreen : Area3D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;

	[Export]
	private StandardMaterial3D renderMat;
	[Export]
	private VideoStreamPlayer videoPlayer;
	[Export]
	private MeshInstance3D liveMesh;
	[Export]
	private MeshInstance3D videoMesh;
	private ImageTexture renderTexture;
	private Image grabbedImage;

	public enum RenderState
	{
		Default,
		LiveDisplay,
		VideoDisplay
	}
	private RenderState state;

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
		state = RenderState.Default;

		Setup();
	}

	
	public override void _Process(double delta)
	{
		HandleInput();
		HandleRenderState();
	}

    // --------------------------------
    //			SETUP LOGIC	
    // --------------------------------

    private void Setup()
	{
		ToggleDisplays(true);
		DisplayServer.WindowSetCurrentScreen(0);
		grabbedImage = DisplayServer.ScreenGetImage(1);
		renderTexture = ImageTexture.CreateFromImage(grabbedImage);
		renderMat.AlbedoTexture = renderTexture;
	}

    // --------------------------------
    //			GENERAL LOGIC
    // --------------------------------

	private void ToggleDisplays(bool isLiveVisible)
	{
        liveMesh.Visible = isLiveVisible;
        videoMesh.Visible = !isLiveVisible;
        videoPlayer.Visible = !isLiveVisible;
    }

	private void HandleInput()
	{
        if (IsReturningToDefault())
        {
			// GD.Print("Trying to return to default");
			ToggleDisplays(true);
			gameManager.ToggleBRB();
			state = RenderState.Default;
			return;
        }
        if (Input.IsActionJustPressed("toggle_screenLive"))
		{
            GD.Print("Trying to toggle live");
            gameManager.ToggleBRB();
			state = RenderState.LiveDisplay;
			return;
        }
        if (Input.IsActionJustPressed("toggle_screenVideo"))
		{
            GD.Print("Trying to toggle video");
            gameManager.ToggleBRB();
            state = RenderState.VideoDisplay;
			return;
		}
    }

	private bool IsReturningToDefault()
	{
		bool isReturningToDefault = (Input.IsActionJustPressed("toggle_screenLive") 
			&& (state == RenderState.LiveDisplay || state == RenderState.VideoDisplay)) 
			|| (Input.IsActionJustPressed("toggle_screenVideo") 
			&& (state == RenderState.VideoDisplay || state == RenderState.LiveDisplay));

		return isReturningToDefault;
    }

    // --------------------------------
    //			RENDER LOGIC
    // --------------------------------

    private void HandleRenderState()
	{
        switch (state)
        {
            case RenderState.Default:
            case RenderState.LiveDisplay:
                RenderDefault();
                break;
            case RenderState.VideoDisplay:
                RenderVideo();
                break;
        }
    }

    private void RenderDefault()
	{
		grabbedImage.Dispose();
		grabbedImage = DisplayServer.ScreenGetImage(1);
		renderTexture.Update(grabbedImage);
	}

	private void RenderVideo()
	{
		ToggleDisplays(false);
    }
}
