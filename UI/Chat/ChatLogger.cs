using Godot;
using Godot.Collections;
using System;
using System.Text.Json.Nodes;

public partial class ChatLogger : Control
{
    private GameManager gameManager;
    private Dictionary<string, Image> cachedImages = new Dictionary<string, Image>();
    private string imageName = "";

    [Export]
    private PackedScene chatLogScene;
    private Array<ChatLog> chatLogs = new Array<ChatLog>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        gameManager = GameManager.Instance;
        gameManager.ChatReceived += HandleChatMessage;
        gameManager.ImageReceived += ReceiveImageRequests;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void HandleChatMessage(string socketMessageString)
    {
        JsonNode parsedBadges = JsonTools.ParseJson(socketMessageString, "data/user/badges");
        JsonNode parsedUserColor = JsonTools.ParseJson(socketMessageString, "data/user/color");
        JsonNode parsedSender = JsonTools.ParseJson(socketMessageString, "data/user/name");
        JsonNode parsedText = JsonTools.ParseJson(socketMessageString, "data/text");
        JsonNode parsedEmotes = JsonTools.ParseJson(socketMessageString, "data/emotes");

        GD.Print($"ChatLogger.cs: Message Text: {parsedText.ToString()}");


        if (parsedText == null || parsedSender == null) return;

        BeginImageRequests(parsedBadges);
        // BeginImageRequests(parsedEmotes);
    }

    private void BeginImageRequests(JsonNode imageUrlNode)
    {
        GD.Print($"ChatLogger.cs: Beginning Image Request");
        JsonNode parsedImageUrl = imageUrlNode[0].GetJsonNodeValueByString("imageUrl");
        string incomingImageName = imageUrlNode[0].GetJsonNodeValueByString("name").ToString();

        if (cachedImages.ContainsKey(incomingImageName))
        {
            // Use Image @ cachedImages[incomingImageName]
        }

        HTTPTool.Instance.PerformHttpRequest(parsedImageUrl.ToString());
        // ++requestCounter;
    }

    private void ReceiveImageRequests(Image receivedImage)
    {
        GD.Print($"ChatLogger.cs: Received Image Request");
        cachedImages.Add(imageName, receivedImage);
        ConstructChatMessage();
    }

    private void ConstructChatMessage()
    {
        ChatLog newChatLog = chatLogScene.Instantiate<ChatLog>();
        AddChild(newChatLog);

        Image currentBadge = cachedImages[imageName];
        ImageTexture texture = ImageTexture.CreateFromImage(currentBadge);

        newChatLog.UpdateChat(texture);
    }
}
