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
	[Export]
	private MeshInstance3D mouseMesh;

	// Mouse Logic
	private Vector2 mouseMinimumPosition;
	private Vector2 mouseMaximumPosition;
	[Export]
	private float mouseZOffset = .05f;

	[Export]
	private int screenIndex = 1;
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
		// state = RenderState.Default;

		Setup();
	}

	
	public override void _Process(double delta)
	{
		//HandleInput();
		//HandleRenderState();
		//renderMat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
	}

    // --------------------------------
    //			SETUP LOGIC	
    // --------------------------------

    private void Setup()
	{
		ToggleDisplays(true);
		DisplayServer.WindowSetCurrentScreen(0);
		//grabbedImage = DisplayServer.ScreenGetImage(screenIndex);
		//renderTexture = ImageTexture.CreateFromImage(grabbedImage);
		//renderMat.AlbedoTexture = renderTexture;

		mouseMinimumPosition = DisplayServer.ScreenGetPosition(screenIndex);
		mouseMaximumPosition = DisplayServer.ScreenGetPosition(screenIndex) + DisplayServer.ScreenGetSize(screenIndex);
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
		RenderMouse();

		if(!gameManager.AllowInput) { return; }

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

	private void RenderMouse()
	{
        // Mouse is always rendered, regardless of input allowance
        if (Input.IsActionJustPressed("toggle_mouse"))
        {
            mouseMesh.Visible = !mouseMesh.Visible;
        }
		if(!mouseMesh.Visible) { return; }

        // Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2 mousePos = DisplayServer.MouseGetPosition();
        
		Vector3 bbSize = liveMesh.Mesh.GetAabb().Size; // 17.072, 9.603
		Vector2 bbRangeMin = new Vector2(liveMesh.GlobalPosition.X - (bbSize.X / 2), liveMesh.GlobalPosition.Y + (bbSize.Y / 2));
        Vector2 bbRangeMax = new Vector2(liveMesh.GlobalPosition.X + (bbSize.X / 2), liveMesh.GlobalPosition.Y - (bbSize.Y / 2));

        Vector3 worldMousePos = new Vector3(
			Remap(mouseMinimumPosition.X, mouseMaximumPosition.X, bbRangeMin.X, bbRangeMax.X, mousePos.X),
			Remap(mouseMinimumPosition.Y, mouseMaximumPosition.Y, bbRangeMin.Y, bbRangeMax.Y, mousePos.Y),
			Position.Z + mouseZOffset 
		);

        //GD.Print(mousePos);

        mouseMesh.GlobalPosition = worldMousePos;
	}

	public static float Remap(float leftAVal, float rightAVal, float leftBVal, float rightBVal, float point)
	{
        float scale1 = rightAVal - leftAVal;
		float scale2 = rightBVal - leftBVal;
		float scaleRatio = scale2 / scale1;
		return ((point - leftAVal) * scaleRatio) + leftBVal;
    }

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
