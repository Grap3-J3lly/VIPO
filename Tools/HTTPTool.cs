using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class HTTPTool : Node
{
    private HttpRequest httpRequest;
    
    public HttpRequest HttpRequest { get => httpRequest; }
    public static HTTPTool Instance { get; private set; }

    public Array<HttpRequest> requests = new Array<HttpRequest>();

    public override void _Ready()
    {
        Instance = this;
        // Create an HTTP request node and connect its completion signal.
        //httpRequest = new HttpRequest();
        //AddChild(httpRequest);
        //httpRequest.RequestCompleted += HttpRequestCompleted;
    }

    public void PerformHttpRequest(string url)
    {
        HttpRequest newRequest = new HttpRequest();
        AddChild(newRequest);
        newRequest.RequestCompleted += HttpRequestCompleted;
        requests.Add(newRequest);

        // Perform the HTTP request. The URL below returns a PNG image as of writing.
        Error error = newRequest.Request(url);
        if (error != Error.Ok)
        {
            GD.PushError("An error occurred in the HTTP request.");
        }

        GD.Print($"HTTPTool.cs: New Request Info? {newRequest}");
    }

    // Called when the HTTP request is completed.
    private void HttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result != (long)HttpRequest.Result.Success)
        {
            GD.PushError("Image couldn't be downloaded. Try a different image.");
        }
        Image image = new Image();
        Error error = image.LoadPngFromBuffer(body);
        if (error != Error.Ok)
        {
            GD.PushError("Couldn't load the image.");
            return;
        }

        // Need to figure out which request is being completed so we can remove it correctly
        // Would "this" work? Since "this" object is what called this function?
        GD.Print($"HTTPTool.cs: Result: {result}, ResponseCode: {responseCode}, Headers: {headers}, Body: {body}");

        GameManager.Instance.EmitSignal(GameManager.SignalName.ImageReceived, image);

        

        //var texture = ImageTexture.CreateFromImage(image);

        //// Display the image in a TextureRect node.
        //var textureRect = new TextureRect();
        //AddChild(textureRect);
        //textureRect.Texture = texture;
    }
}
