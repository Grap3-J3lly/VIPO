using Godot;
using System;
using System.Threading.Tasks;

public partial class WSClient : Node
{
	// --------------------------------
	//			VARIABLES	
	// --------------------------------

	[Export]
	private string connectionUrl = "ws://localhost:8080";

    [Export]
	private string[] handshakeHeaders;
	[Export]
	private string[] supportedProtocols;
	private TlsOptions tlsOptions = null;
	private WebSocketPeer socket = new WebSocketPeer();
	private WebSocketPeer.State lastState = WebSocketPeer.State.Closed;

    // --------------------------------
    //			SIGNALS	
    // --------------------------------

    [Signal]
	public delegate void ConnectedToServerEventHandler();
    [Signal]
    public delegate void ConnectionClosedEventHandler();
	[Signal]
	public delegate void MessageReceivedEventHandler(Variant message);

    // --------------------------------
    //		CONNECTION LOGIC
    // --------------------------------
	private int ConnectToUrl(string url)
	{
		socket.SupportedProtocols = supportedProtocols;
		socket.HandshakeHeaders = handshakeHeaders;
		Error err = socket.ConnectToUrl(url, tlsOptions);
		if (err != Error.Ok)
		{
			return (int)err;
		}

		lastState = socket.GetReadyState();
		return (int)Error.Ok;
	}

	public int Send(string message)
	{
		return (int)socket.SendText(message);
	}

	private Variant GetMessage()
	{
		Variant message = new Variant();
		if(socket.GetAvailablePacketCount() < 1)
		{
			return message;
		}
		byte[] pkt = socket.GetPacket();

		if(socket.WasStringPacket())
		{
			// GD.Print(pkt.GetStringFromUtf8());
			return pkt.GetStringFromUtf8();
		}
		return pkt;
	}

	private void Close(int code = 1000, string reason = "")
	{
		socket.Close(code, reason);
		lastState = socket.GetReadyState();
	}

	private void Clear()
	{
		socket = new WebSocketPeer();
		lastState = socket.GetReadyState();
	}

	private WebSocketPeer GetSocket()
	{
		return socket;
	}

	private void Poll()
	{
		if(socket.GetReadyState() != WebSocketPeer.State.Closed)
		{
			socket.Poll();
		}

		WebSocketPeer.State state = socket.GetReadyState();

		if (lastState != state)
		{
			lastState = state;
			if (state == WebSocketPeer.State.Open)
			{
				EmitSignal(SignalName.ConnectedToServer);
				GD.Print($"WSClient.cs: Websocket Connected");
			}
			else if (state == WebSocketPeer.State.Closed)
			{
				EmitSignal(SignalName.ConnectionClosed);
			}
		}
        while (socket.GetReadyState() == WebSocketPeer.State.Open && socket.GetAvailablePacketCount() > 0)
        {
            EmitSignal(SignalName.MessageReceived, GetMessage());
            //GD.Print($"WSClient.cs: MessageReceived: {GetMessage().ToString()}");
        }
    }

	public static string DoAction(string actionName)
	{
        return
        """
        {
            "request": "DoAction",
            "action": {
                "name": "
        """
        +
        actionName
        +
        """ 
        "
            },
            "id": "<id>"
        }
        """;
    }

	public static string GetGlobal(string globalVarName)
	{
        return
        """
		{
			"request": "GetGlobal",
			"id": "my-subscribe-id",
			"variable": "
		"""
            +
			globalVarName
			+
		"""
		",
			"persisted": "true",
		}
		""";
    }

	public static string GetGlobals()
	{
        return
        """
		{
			"request": "GetGlobals",
			"id": "my-subscribe-id",
			"persisted": "true",
		}
		""";
    }

    public override void _Ready()
    {
		GetTree().AutoAcceptQuit = false;

        if(socket.ConnectToUrl(connectionUrl) != Error.Ok)
		{
			GD.Print($"WSClient.cs: Unable to connect");
			SetProcess(false);
		}


		ConnectedToServer += () => Send(
			"""
			{
				"request": "Subscribe",
				"id": "my-subscribe-id",
				"events": 
				{
					"raw": 
					[
						"Action"
					],
					"twitch":
					[
						"ChatMessage"
					]
				}
			}
			"""
		);
	// ConnectedToServer += () => Send(DoAction("EnableWheelRewards"));

	}

	public override void _Process(double delta)
	{
		Poll();
	}

    public override void _Notification(int what)
    {
        base._Notification(what);
		if(what == NotificationWMCloseRequest)
		{
			OnQuit();
		}
    }

    public async Task OnQuit()
    {
		GD.Print($"WSClient.cs: Running On Quit");
		Send(DoAction("DisableWheelRewards"));
        await ToSignal(GetTree().CreateTimer(.1), SceneTreeTimer.SignalName.Timeout);
        GetTree().Quit();
    }

	
}