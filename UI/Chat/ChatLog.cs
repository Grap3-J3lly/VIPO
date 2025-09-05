using Godot;
using System;

[GlobalClass]
public partial class ChatLog : RichTextLabel
{
	private GameManager gameManager;
	public override void _Ready()
	{
		gameManager = GameManager.Instance;
		gameManager.UpdateChatTexture += UpdateChat;
	}

	
	public override void _Process(double delta)
	{
	}

	public void UpdateChat(string newChat)
	{
		Text += newChat;
	}

	public void UpdateChat(Texture2D texture)
	{
		AddImage(texture);
	}
}
