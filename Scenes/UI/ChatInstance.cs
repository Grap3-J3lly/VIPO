using Godot;
using System;

public partial class ChatInstance : VBoxContainer
{
	[Export]
	private RichTextLabel chatUserName;
	[Export]
	private RichTextLabel chatText;
	[Export]
	private Color defaultColor;

    public override void _Ready()
    {
        base._Ready();
		SetChatUserColor(defaultColor);
    }

    public void SetChatUserColor(Color color)
	{
		chatUserName.AddThemeColorOverride("default_color", color);
	}
	
	public void SetChatUserName(string nameVal)
	{
		chatUserName.Text = nameVal +":";
	}

	public void SetChatText(string textVal)
	{
		chatText.Text = textVal;
	}
}
