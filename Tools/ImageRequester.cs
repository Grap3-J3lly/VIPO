using Godot;
using System;
using System.Collections.Generic;

public partial class ImageRequester : Node
{
    private static ImageRequester Instance;

    // Things To Include:
    // - Image Cache, once an image has been downloaded once, it is reused
    // - Request Cache, to track and handle multiple requests for the same image at the same time (if image is not in cache yet)
    // - Guarantee Correct Image tied to callback
    private Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();
    private Dictionary<string, Request> requestCache = new Dictionary<string, Request>();

    // The ImageRequester then caches the image with the link as the key,
    // cleans up the Requester obj and then calls any callbacks that were
    // collected from any and all objects that requested that image.

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }
    

	private class Request
	{
        // That Request object knows which image it is expecting and is therefore
        // able to re-associate the image's link with the new image data.
        // Once that is done it calls the non-unique callback on the ImageRequester
        // node with two pieces of data: The link for the image and the image texture itself.

        private HttpRequest httpRequest;
        private string requestUrl;

        // Events


        public Request(string url)
        {
            // HttpRequest gets a response from whatever server is serving the image,
            // it then calls the callback on the unique Request object that is subscribed to it.
            httpRequest = new HttpRequest();
            Instance.AddChild(httpRequest);
            httpRequest.RequestCompleted += HttpImageRequestCompleted;

            requestUrl = url;
            Error error = httpRequest.Request(url);
            if (error != Error.Ok)
            {
                GD.PushError("An error occurred in the HTTP request.");
            }
        }

        // Called when the HTTP request is completed.
        private void HttpImageRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
        {
            if (result != (long)HttpRequest.Result.Success)
            {
                GD.PushError("Image couldn't be downloaded. Try a different image.");
            }

            Image image = LoadImageByType(headers, body);
            
        }

        private Image LoadImageByType(string[] imageTypeInfo, byte[] imageData)
        {
            Image image = new Image();
            string imageType = "";
            Error error = Error.Failed;
            foreach (string data in imageTypeInfo)
            {
                if (data.StartsWith("Content-Type: image/"))
                {
                    imageType = data.Substring(20);

                    switch (imageType)
                    {
                        case "bmp":
                            error = image.LoadBmpFromBuffer(imageData);
                            break;
                        case "jpeg":
                            error = image.LoadJpgFromBuffer(imageData);
                            break;
                        case "webp":
                            error = image.LoadWebpFromBuffer(imageData);
                            break;
                        case "png":
                            error = image.LoadPngFromBuffer(imageData);
                            break;
                    }

                    if (error != Error.Ok)
                    {
                        GD.PushError("Couldn't load the image.");
                        return null;
                    }
                }
            }
            return image;
        }
    }
}
