using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;


public partial class ChatLogger : Control
{
    private GameManager gameManager;
    private Godot.Collections.Dictionary<string, Image> cachedImages = new Godot.Collections.Dictionary<string, Image>();

    [Export]
    private PackedScene chatLogScene;
    private List<ChatLog> chatLogs = new List<ChatLog>();

    public List<(HttpRequest, string)> requests = new List<(HttpRequest request, string imageName)>();

    private ChatLog currentChatLog;

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

    // ChatLogger->ImageRequester->Request->HttpRequest
    

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
        BeginImageRequests(parsedEmotes);

        currentChatLog = ConstructChatMessage();
    }

    private void BeginImageRequests(JsonNode imageUrlNode)
    {
        GD.Print($"ChatLogger.cs: Beginning Image Request");
        JsonNode parsedImageUrl = null;
        string incomingImageName = "";
        HttpRequest newRequest;

        for(int nodeIndex = 0; nodeIndex < imageUrlNode.AsArray().Count; nodeIndex++)
        {
            parsedImageUrl = imageUrlNode[nodeIndex].GetJsonNodeValueByString("imageUrl");
            incomingImageName = imageUrlNode[nodeIndex].GetJsonNodeValueByString("name").ToString();
            //imageName = incomingImageName;
            // newRequest = Request.Instance.PerformHttpImageRequest(parsedImageUrl.ToString());

            GD.Print($"ChatLogger.cs: Incoming ImageName: {incomingImageName}");
            // requests.Add((newRequest, incomingImageName));
        }

        //if (cachedImages.ContainsKey(incomingImageName))
        //{
        //    // Use Image @ cachedImages[incomingImageName]
        //}

        // ++requestCounter;
    }

    private void ReceiveImageRequests(Image receivedImage)
    {
        string receivedImageName = "";
        foreach ((HttpRequest request, string imageName) in requests)
        {
            if (request.GetHttpClientStatus() == HttpClient.Status.Disconnected)
            {
                GD.Print($"ChatLogger.cs: Image Name: {imageName}");

                receivedImageName = imageName;
                cachedImages.Add(receivedImageName, receivedImage);
                
                UpdateChatMessage(receivedImageName);

                requests.Remove((request, imageName));
                break;
            }
        }

    }

    private ChatLog ConstructChatMessage()
    {
        ChatLog newChatLog = chatLogScene.Instantiate<ChatLog>();
        AddChild(newChatLog);

        return newChatLog;
    }

    private void UpdateChatMessage(string imageName)
    {
        Image currentBadge = cachedImages[imageName];
        ImageTexture texture = ImageTexture.CreateFromImage(currentBadge);

        currentChatLog.UpdateChat(texture);
    }
}
