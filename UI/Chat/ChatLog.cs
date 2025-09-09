using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class ChatLog : RichTextLabel
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;

	private Array<Image> badges;
	private Array<Image> emotes;
	private string userDisplayName = "";
	private Color userColor = Colors.Black;
	private string chatMessage;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public Array<Image> Badges { get => badges; set => badges = value; }
	public Array<Image> Emotes { get => emotes; set => emotes = value; }
	public string UserDisplayName { get => userDisplayName; set => userDisplayName = value; }
	public Color UserColor { get => userColor; set => userColor = value; }
	public string ChatMessage { get => chatMessage; set => chatMessage = value; }


    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
		gameManager.UpdateChatTexture += UpdateChat;
		FitContent = true;
	}

	
	public override void _Process(double delta)
	{
	}

    // --------------------------------
    //			CHAT LOGIC	
    // --------------------------------

    public void UpdateChat(string newChat)
	{
		Text += newChat;
	}

	public void UpdateChat(Texture2D texture)
	{
		AddImage(texture);
	}
}
