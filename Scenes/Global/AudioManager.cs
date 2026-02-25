using Godot;
using System;

public partial class AudioManager : Node
{

	[Export]
	private AudioStreamPlayer mic_input;
	[Export]
	private float mic_VolumeCutoff = -125f;

	public static AudioManager Instance { get; private set; }

	public override void _Ready()
	{
		base._Ready();
		Instance = this;
	}

	public bool IsCapturingAudio()
	{
        float inputVolume = AudioServer.GetBusPeakVolumeLeftDb(AudioServer.GetBusIndex("Mic"), 0);
		return inputVolume >= mic_VolumeCutoff;
    }
}