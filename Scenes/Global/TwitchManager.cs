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

    // Action Details
    public enum TwitchActions
    {
        Undefined = -1,
        Enlarge = 0,
        Reduce = 1,
        Scry = 2,
        FindFamiliar = 3
    }
    [Export]
    private Godot.Collections.Dictionary<string, TwitchActions> availableActions = new Godot.Collections.Dictionary<string, TwitchActions>();

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
        
        // wsClient.ConnectedToServer += OnConnection;
        wsClient.MessageReceived += OnWebSocketMessage;
        CallDeferred("Setup");
    }

    // --------------------------------
    //	   UNFILTERED LOGIC
    // --------------------------------

    private void Setup()
    {
        gameManager = GameManager.Instance;
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
        JsonNode parsedUserName = JsonTools.ParseJson(socketMessageString, "data/arguments/userName");
        string action = parsedAction.ToString();
        string userName = parsedUserName.ToString();
        GD.Print($"TwitchManager.cs: Action Called: {action}");
        ProcessUserActions(action, userName);

    }

    private void ProcessUserActions(string actionName, string userName)
	{
        TwitchActions currentAction = TwitchActions.Undefined;
        if(availableActions.ContainsKey(actionName))
        {
            currentAction = availableActions[actionName];
        }

		switch(currentAction)
		{
			case TwitchActions.Enlarge:
				EventManager.Instance.ChangeScaleEventEmit(isIncreasing: true);
			break;
			case TwitchActions.Reduce:
				EventManager.Instance.ChangeScaleEventEmit(isIncreasing: false);
			break;
			case TwitchActions.Scry:
				EventManager.Instance.DisplayScryScreenEventEmit(enable: true);
			break;
			case TwitchActions.FindFamiliar:
				EventManager.Instance.TrySpawnFamiliarEventEmit(userName);
			break;
			default:
				GD.PrintErr($"EventManager.cs: Action Does Not Exist");
			break;
		}
	}

    // --------------------------------
    //		    CHAT LOGIC	
    // --------------------------------


}
