using Godot;
using System;

public partial class AIManager : Node
{
	public static AIManager Instance;
	[Export]
	private float mapCellSize = 10;
	[Export]
	private float mapCellHeight = 5;
	private Rid mainRID;

	public override void _Ready()
	{
		Instance = this;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
