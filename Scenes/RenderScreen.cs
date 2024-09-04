using Godot;
using System;

public partial class RenderScreen : Area3D
{
	[Export]
	private StandardMaterial3D renderMat;
	private ImageTexture renderTexture;
	private Image grabbedImage;

	public override void _Ready()
	{
		DisplayServer.WindowSetCurrentScreen(0);
		grabbedImage = DisplayServer.ScreenGetImage(1);
		renderTexture = ImageTexture.CreateFromImage(grabbedImage);
		renderMat.AlbedoTexture = renderTexture;
	}

	
	public override void _Process(double delta)
	{
		grabbedImage.Dispose();
		grabbedImage = DisplayServer.ScreenGetImage(1);
		renderTexture.Update(grabbedImage);
	}
}
