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
	public delegate void PopulateScryAreaDataEventHandler(Dictionary<Camera3D, Node3D> scryAreas);
	[Signal]
	public delegate void TrySpawnFamiliarEventHandler(string userName);

	// Input-Related Signals
	[Signal]
	public delegate void EnableMovementEventHandler(bool enable);
	[Signal]
	public delegate void EnableInputEventHandler(bool enable);
	[Signal]
	public delegate void SpeedChangeEventHandler(float newSpeed);

	// Menu Signals
	[Signal]
	public delegate void ResetEventHandler(bool value);

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

	public void PopulateScryAreaDataEventEmit(Dictionary<Camera3D, Node3D> scryAreas)
	{
		EmitSignal(SignalName.PopulateScryAreaData, scryAreas);
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

	public void SpeedChangeEventEmit(float newSpeed)
	{
		GD.Print($"EventManager.cs: Firing Speed Change Event");
		EmitSignal(SignalName.SpeedChange, newSpeed);
	}

	// Menu-Related Emissions

	public void ResetEventEmit(bool value = true)
	{
		EmitSignal(SignalName.Reset, value);
	}

}
