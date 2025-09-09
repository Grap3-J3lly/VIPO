using Godot;
using Godot.Collections;
using System;


[GlobalClass]
public partial class HTTPTool : Node
{
    private HttpRequest httpRequest;
    
    public HttpRequest HttpRequest { get => httpRequest; }
    public static HTTPTool Instance { get; private set; }

    // Needed a means of tying the image name to the specific HTTPRequest in order to tie the loaded Image to a key in the loaded Image cache in ChatLogger
    public Array<HttpRequest> requests = new Array<HttpRequest>();

    public override void _Ready()
    {
        Instance = this;
        // Create an HTTP request node and connect its completion signal.
        //httpRequest = new HttpRequest();
        //AddChild(httpRequest);
        //httpRequest.RequestCompleted += HttpRequestCompleted;
    }

    public HttpRequest PerformHttpImageRequest(string url)
    {
        HttpRequest newRequest = new HttpRequest();
        AddChild(newRequest);
        newRequest.RequestCompleted += HttpImageRequestCompleted;
        requests.Add(newRequest);

        Error error = newRequest.Request(url);
        if (error != Error.Ok)
        {
            GD.PushError("An error occurred in the HTTP request.");
        }

        return newRequest;
    }

    // Called when the HTTP request is completed.
    private void HttpImageRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
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

        foreach (HttpRequest request in requests)
        {
            
            HttpClient.Status status = request.GetHttpClientStatus();
            if(status == HttpClient.Status.Disconnected)
            {
                requests.Remove(request);
                request.QueueFree();
                break;
            }
        }

        //var texture = ImageTexture.CreateFromImage(image);

        //// Display the image in a TextureRect node.
        //var textureRect = new TextureRect();
        //AddChild(textureRect);
        //textureRect.Texture = texture;
    }
}
