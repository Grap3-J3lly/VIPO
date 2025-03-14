using Godot;
using Godot.Collections;
using System;

public partial class ObjectPool : Node
{
    public Array<Node3D> objects = new Array<Node3D>();

    [Export]
    private PackedScene familiarScene;

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
    }
}
