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

    private IEnumerator MakePredictionRequest(byte[] byteData)
    {
        Debug.Log("Got into prediction method");
        WWWForm webForm = new WWWForm();
        using (UnityWebRequest uwr = UnityWebRequest.Post(Settings.ApiURL, webForm))
        {
            uwr.SetRequestHeader("Content-Type", Settings.ContentType);
            uwr.SetRequestHeader("Prediction-Key", Settings.PredictionKey);

            uwr.uploadHandler = new UploadHandlerRaw(byteData);
            uwr.uploadHandler.contentType = Settings.ContentType;

            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            Debug.Log("Sending prediction request");
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                var responseMessage = uwr.downloadHandler.text;
                var resposeData = uwr.downloadHandler.data;

                Debug.Log("Received: " + responseMessage);
            }
        }
    }

    private static byte[] GetImageAsByteArray(Texture2D texture)
    {  
        return ImageConversion.EncodeToJPG(texture);
    }
}
