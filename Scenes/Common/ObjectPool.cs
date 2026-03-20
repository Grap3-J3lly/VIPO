using Godot;
using Godot.Collections;
using System;

public partial class ObjectPool : Node
{
    private Node3D characterController;
    public Array<Node3D> objects = new Array<Node3D>();

    [Export]
    private PackedScene familiarScene;

    public static ObjectPool Instance { get; private set; }

    public CharacterController CharacterController { get => (CharacterController)characterController.GetChild(0); }

    public override void _Ready()
    {
        Instance = this;
        EventManager.Instance.TrySpawnFamiliar += TrySpawnFamiliar;

        characterController = (Node3D)GameManager.Instance.CharControllerScene.Instantiate();
		AddChild(characterController);
		objects.Add(characterController);
    }

    public void TrySpawnFamiliar(string userName)
    {
        foreach(Node3D node in objects)
        {
            try
            {
                if (((Familiar)node).FamiliarName == userName)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                continue;
            }
        }

        Familiar familiar = (Familiar)familiarScene.Instantiate();
        AddChild(familiar);
        objects.Add(familiar);
        familiar.UpdateName(userName);
        if(userName == "Gandalf")
        {
            familiar.MakeGandalf();
        }
    }
}
