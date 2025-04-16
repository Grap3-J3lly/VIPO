using Godot;
using System;

public partial class Familiar : CharacterBody3D
{
    private GameManager gameManager;
    private NavigationAgent3D navAgent;
    private CharacterController charController;
    private Label3D nameTextField;

    private string familiarName = "Default Name";

    [Export]
    private Vector3 initialTargetPosition = new Vector3(10, 0, 5);

    [Export]
    private float maxRangeFromTarget = 10.0f;

    [Export]
	public float speed = 5.0f;

    private MeshInstance3D tempMesh;
    [Export]
    private StandardMaterial3D defaultMaterial;
    [Export]
    private StandardMaterial3D gandalfMaterial;

    [Export]
    private Color defaultColor;
    [Export]
    private Color gandalfColor;

    private bool pause = false;
    private float timer = 0;
    [Export]
    private float moveDelay = 1f;

    // --------------------------------
    //		    PROPERTIES
    // --------------------------------

    public string FamiliarName { get => familiarName; set => familiarName = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        base._Ready();
        gameManager = GameManager.Instance;
        navAgent = this.FindFirstChildOfType<NavigationAgent3D>();
        charController = gameManager.CharacterController.FindFirstChildOfType<CharacterController>();
        nameTextField = this.FindFirstChildOfType<Label3D>();
        tempMesh = this.FindFirstChildOfType<MeshInstance3D>();
        AssignRandomDestinationAroundTarget();
        navAgent.NavigationFinished += AssignRandomDestinationAroundTarget;
        navAgent.NavigationFinished += PauseMovement;

        tempMesh.Mesh.SurfaceSetMaterial(0, defaultMaterial);
        // defaultMaterial.AlbedoColor = defaultColor;
    }

    public override void _PhysicsProcess(double delta)
	{       
        Vector3 nextPathPosition = navAgent.GetNextPathPosition();
        if(pause)
        {
            timer -= (float)delta;
            if(timer <= 0)
            {
                pause = false;
            }
            return;
        }
        
        Move(nextPathPosition);
    }

    // --------------------------------
    //		GENERAL LOGIC	
    // --------------------------------

    public void UpdateName(string newName)
    {
        familiarName = newName;
        nameTextField.Text = newName;
    }

    public void MakeGandalf()
    {
        tempMesh.Mesh = (Mesh)tempMesh.Mesh.Duplicate();
        tempMesh.Mesh.SurfaceSetMaterial(0, gandalfMaterial);
        // defaultMaterial.AlbedoColor = gandalfColor;
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

    private void PauseMovement()
    {
        pause = true;
        timer = moveDelay;
    }

    private void AssignRandomDestinationAroundTarget()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        Vector3 randomPosition = charController.Position;
        randomPosition.X += rand.RandfRange(-maxRangeFromTarget, maxRangeFromTarget);
        randomPosition.Z += rand.RandfRange(-maxRangeFromTarget, maxRangeFromTarget);

        GD.Print("Assigned Location: " + randomPosition);
                
        navAgent.TargetPosition = randomPosition;
    }

    private void Move(Vector3 nextLocation)
    {
        Vector3 velocity = Velocity;

        try
        {
            LookAt(nextLocation);
        }
        catch (Exception e)
        {
            GD.Print("Familiar.Move - Node and target are in same position.");
        }

        velocity = -Basis.Z * speed;

        Velocity = velocity;
        MoveAndSlide();
        
    }
}
