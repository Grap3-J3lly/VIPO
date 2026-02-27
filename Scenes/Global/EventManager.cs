using Godot;
using Godot.Collections;
using System;

public partial class EventManager : Node
{
	public static EventManager Instance { get; private set; }
	
	// --------------------------------
	//		USER ACTION SIGNALS	
    // --------------------------------
	[Signal]
	public delegate void ChangeScaleEventHandler(bool isIncreasing);
	[Signal]
	public delegate void DisplayScryScreenEventHandler(bool enable);
	[Signal]
	public delegate void TrySpawnFamiliarEventHandler(string userName);

	// --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

	public override void _Ready()
	{
		Instance = this;

		
	}
	// --------------------------------
	//		USER ACTION EMISSIONS	
    // --------------------------------

	public void ChangeScaleEventEmit(bool isIncreasing)
	{
		EmitSignal(SignalName.ChangeScale, isIncreasing);
	}

	public void DisplayScryScreenEventEmit(bool enable = true)
	{
		EmitSignal(SignalName.DisplayScryScreen, enable);
	}

	public void TrySpawnFamiliarEventEmit(string userName)
	{
		EmitSignal(SignalName.TrySpawnFamiliar, userName);
	}

}
