using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class PatternDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SearchForPattern(Texture2D cameraTexture)
    {
        var imageByteArray = GetImageAsByteArray(cameraTexture);

        Debug.Log("Touch detected - sending request");

        StartCoroutine(MakePredictionRequest(imageByteArray));
    }

    //private static async IEnumerable<HttpResponseMessage> MakePredictionRequest(byte[] byteData)
    //{
    //    var client = new HttpClient();

    //    client.DefaultRequestHeaders.Add("Prediction-Key", Settings.PredictionKey);

    //    HttpResponseMessage response;

    //    using (var content = new ByteArrayContent(byteData))
    //    {
    //        content.Headers.ContentType = new MediaTypeHeaderValue(Settings.ContentType);

    //        response = await client.PostAsync(Settings.ApiURL, content);

    //        Debug.Log(response);
    //    }

    //    yield return response;
    //}

    private IEnumerator MakePredictionRequest(byte[] byteData)
    {
        Debug.Log("Got into prediction method");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Settings.ApiURL, "POST"))
        {
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(byteData);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", Settings.ContentType);
            uwr.SetRequestHeader("Prediction-Key", Settings.PredictionKey);

            Debug.Log("Sending prediction request");
            Console.WriteLine("Sending prediction request");
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
                Console.WriteLine(uwr.error);
            }
            else
            {
                var responseMessage = uwr.downloadHandler.text;
                var resposeData = uwr.downloadHandler.data;

                Debug.Log("Received: " + responseMessage);
                Console.WriteLine(responseMessage);
            }
        }
    }

    private static byte[] GetImageAsByteArray(Texture2D texture)
    {
        return texture.GetRawTextureData();
    }
}
