using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json;

public partial class TwitchManager : Node
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    [Export]
    private WSClient wsClient;
    private GameManager gameManager;

    private int requestCounter = 0;
    private Queue<Image> loadedImages = new Queue<Image>();

    // Chat Details
    private JsonNode parsedBadges;
    private JsonNode parsedUserColor;
    private JsonNode parsedSender;
    private JsonNode parsedText;
    private JsonNode parsedEmotes;

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        gameManager = GameManager.Instance;
        // wsClient.ConnectedToServer += OnConnection;
        wsClient.MessageReceived += OnWebSocketMessage;
        gameManager.ToggleTwitch += ToggleInteractions;
    }

    // --------------------------------
    //	   TWITCH CONNECTION LOGIC	
    // --------------------------------

    /// <summary>
    /// Toggles the variable to listen to the chat or not
    /// Upon toggling off, clears the tracked list of users
    /// </summary>
    private void OnConnection()
    {
    }

    public void ToggleInteractions(bool isActive)
    {
        if (isActive)
        {
            wsClient.MessageReceived += OnWebSocketMessage;
        }
        else
        {
            wsClient.MessageReceived -= OnWebSocketMessage;
        }
    }

    /// <summary>
    /// Checks the incoming chat logs to determine if a vote was made on an existing option and updates the weight if the user has not already voted previously
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    private void OnWebSocketMessage(Variant message)
    {
        string socketMessageString = message.ToString();
        // GD.Print($"TwitchManager.cs: SocketMessageString: {socketMessageString}");
        JsonNode checkText = JsonTools.ParseJson(socketMessageString, "event/type");

        if (checkText?.ToString() == "Action")
        {
            HandleAction(socketMessageString);
            return;
        }
        if (checkText?.ToString() == "ChatMessage")
        {
            gameManager.EmitSignal(GameManager.SignalName.ChatReceived, socketMessageString);
            return;
        }
    }

    // --------------------------------
    //		    ACTION LOGIC	
    // --------------------------------

    private void HandleAction(string socketMessageString)
    {
        JsonNode parsedAction = JsonTools.ParseJson(socketMessageString, "data/arguments/actionName");
        GD.Print($"TwitchManager.cs: Action Called: {parsedAction.ToString()}");
    }

    // --------------------------------
    //		    CHAT LOGIC	
    // --------------------------------


}
