using Godot;
using System;

public partial class InputBasedHandheld : Node3D
{
	[Export]
    private double handHeldSwapTimerDuration = 5;
	private double timer_handheldDisplayReset;

	[Export]
	private Node3D primaryInputCosmetic;
	[Export]
	private Node3D secondaryInputCosmetic;
	
	private bool movementEnabled = true;
	private bool inputEnabled = true;

    // Getters/Setters - For connecting to Signals bc C# Properties can't be attached to Signals
    public void SET_MovementEnabled(bool moveEnable)
    { movementEnabled = moveEnable; }
    public bool GET_MovementEnabled()
    { return movementEnabled; }

    public void SET_InputEnabled(bool inputEnable)
    { inputEnabled = inputEnable; }
    public bool GET_InputEnabled()
    { return inputEnabled; }

    public bool MovementEnabled { get => movementEnabled; set => movementEnabled = value; }

	public override void _Ready()
	{
		timer_handheldDisplayReset = handHeldSwapTimerDuration;
		Visible = false;

		EventManager.Instance.EnableMovement += SET_MovementEnabled;
        EventManager.Instance.EnableInput += SET_InputEnabled;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		InputChecks(delta);
    }

	private void InputChecks(double delta)
	{
		if (!inputEnabled || !movementEnabled)
		{
			if (Input.IsAnythingPressed())
            {
                ToggleInputCosmeticVisibility(2);
                timer_handheldDisplayReset = handHeldSwapTimerDuration;
            }
            else if (secondaryInputCosmetic.Visible == false || timer_handheldDisplayReset <= 0)
            {
                ToggleInputCosmeticVisibility(1);
            }

            if (timer_handheldDisplayReset > 0)
            {
                timer_handheldDisplayReset -= delta;
            }
		}
		else
		{
			ToggleInputCosmeticVisibility(0);
		}
	}

	/// <summary>
    /// When movement is not allowed, display input source cosmetic in front of the character
    /// 0 for nothing, 1 for KB&M, 2 for Controller
    /// </summary>
    private void ToggleInputCosmeticVisibility(int inputNumber = 0)
    {
        Visible = true;
        switch (inputNumber)
        {
            case 0:
            default: // Turn all off
                // primaryInputCosmetic.Visible = false;
                // secondaryInputCosmetic.Visible = false;
                Visible = false;
                break;
            case 1: // KB&M Only
                primaryInputCosmetic.Visible = true;
                secondaryInputCosmetic.Visible = false;
                break;
            case 2: // Controller Only
                primaryInputCosmetic.Visible = false;
                secondaryInputCosmetic.Visible = true;
                break;
        }
    }
}
