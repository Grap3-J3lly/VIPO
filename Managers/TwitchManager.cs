using Godot;
// using Godot.Collections;
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
        GD.Print($"TwitchManager.cs: SocketMessageString: {socketMessageString}");
        JsonNode checkText = ParseJson(socketMessageString, "event/type");

        if (checkText?.ToString() == "Action")
        {
            HandleAction(socketMessageString);
            return;
        }
        if (checkText?.ToString() == "ChatMessage")
        {
            HandleChatMessage(socketMessageString);
            return;
        }
    }

    private JsonNode ParseJson(string messageToParse, string dataPath)
    {
        JsonNode root = JsonNode.Parse(messageToParse);

        if (root == null)
        {
            GD.Print($"ToggleChatInputButton.cs: Failed to Parse");
            return null;
        }

        JsonNode result = root.GetJsonNodeValueByString(dataPath);

        return result;
    }

    // --------------------------------
    //		    ACTION LOGIC	
    // --------------------------------

    private void HandleAction(string socketMessageString)
    {
        JsonNode parsedAction = ParseJson(socketMessageString, "data/arguments/actionName");
        GD.Print($"TwitchManager.cs: Action Called: {parsedAction.ToString()}");
    }

    // --------------------------------
    //		    CHAT LOGIC	
    // --------------------------------

    private void HandleChatMessage(string socketMessageString)
    {
        // Need to handle breakdown and building of chat message in Twitch Manager,
        // make ChatLog object on RichTextLabel handle formatting of chat messages
        
        // Example Message: [Badges][parsedSender(colored using parsedUserColor)]: [parsedText] + [parsedEmotes (placed where stated in JSON)]
        // To make images display in RichTextLabel: enable bbcode, wrap in [img]http://imgLink.com/ [/img]

        JsonNode parsedBadges = ParseJson(socketMessageString, "data/user/badges");
        JsonNode parsedUserColor = ParseJson(socketMessageString, "data/user/color");
        JsonNode parsedSender = ParseJson(socketMessageString, "data/user/name");
        JsonNode parsedText = ParseJson(socketMessageString, "data/text");
        JsonNode parsedEmotes = ParseJson(socketMessageString, "data/emotes");

        GD.Print($"ToggleChatInputButton.cs: Message Text: {parsedText.ToString()}");


        if (parsedText == null || parsedSender == null) return;

        ConstructChatMessage(parsedBadges, parsedUserColor, parsedSender, parsedText, parsedEmotes);
    }

    private void ConstructChatMessage(JsonNode parsedBadges, JsonNode parsedUserColor, JsonNode parsedSender, JsonNode parsedText, JsonNode parsedEmotes)
    {

    }
}
