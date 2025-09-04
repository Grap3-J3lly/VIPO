using Godot;
using System;

[GlobalClass]
public partial class HTTPTool : HttpRequest
{
    private HttpRequest httpRequest;
    
    public HttpRequest HttpRequest { get => httpRequest; }
    public static HTTPTool Instance { get; private set; }


    public override void _Ready()
    {
        Instance = this;
        // Create an HTTP request node and connect its completion signal.
        httpRequest = new HttpRequest();
        AddChild(httpRequest);
        httpRequest.RequestCompleted += HttpRequestCompleted;
    }

    public void PerformHttpRequest(string url)
    {
        // Perform the HTTP request. The URL below returns a PNG image as of writing.
        Error error = httpRequest.Request(url);
        if (error != Error.Ok)
        {
            GD.PushError("An error occurred in the HTTP request.");
        }
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

        GameManager.Instance.EmitSignal(GameManager.SignalName.ImageReceived, image);

        //var texture = ImageTexture.CreateFromImage(image);

        //// Display the image in a TextureRect node.
        //var textureRect = new TextureRect();
        //AddChild(textureRect);
        //textureRect.Texture = texture;
    }
}
