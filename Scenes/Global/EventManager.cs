using Godot;
using Godot.Collections;
using System;

public partial class EventManager : Node
{
	public static EventManager Instance { get; private set; }
	
	// --------------------------------
	//			SIGNALS	
    // --------------------------------

	// UserAction Signals
	[Signal]
	public delegate void ChangeScaleEventHandler(bool isIncreasing);
	[Signal]
	public delegate void DisplayScryScreenEventHandler(bool enable);
	[Signal]
	public delegate void TrySpawnFamiliarEventHandler(string userName);

	// Input-Related Signals
	[Signal]
	public delegate void EnableMovementEventHandler(bool enable);
	[Signal]
	public delegate void EnableInputEventHandler(bool enable);

	// --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

	public override void _Ready()
	{
		Instance = this;

		
	}
	// --------------------------------
	//			EMISSIONS	
    // --------------------------------

	// UserAction Emissions
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

	// Input-Related Emissions
	public void EnableMovementEventEmit(bool enable)
	{
		EmitSignal(SignalName.EnableMovement, enable);
	}

	public void EnableInputEventEmit(bool enable)
	{
		EmitSignal(SignalName.EnableInput, enable);
	}
}
