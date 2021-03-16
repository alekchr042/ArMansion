using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PatternDetector : MonoBehaviour
{
    private RecognitionResponse lastRecognition;
    private ARRaycastManager raycastManager;


    public GameObject PrefabToPlace;

    public double ProbabilityTreshold;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SearchForObject(Texture2D cameraTexture)
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

                var recognition = GetRecognitionResponse(responseMessage);

                if (recognition != null && IsObjectRecognized(recognition))
                {
                    lastRecognition = recognition;

                    PlaceObjectInDetectedPlace();
                }

                Debug.Log("Received: " + responseMessage);
            }
        }
    }

    private byte[] GetImageAsByteArray(Texture2D texture)
    {
        return ImageConversion.EncodeToJPG(texture);
    }

    private RecognitionResponse GetRecognitionResponse(string recognitionResponseAsText)
    {
        var recognitionResponse = JsonUtility.FromJson<RecognitionResponse>(recognitionResponseAsText);

        return recognitionResponse;
    }

    private RecognitionResponse GetLastRecognition()
    {
        return lastRecognition;
    }

    private void PlaceObjectInDetectedPlace()
    {
        var placedObject = Instantiate(PrefabToPlace);

        var bounds = placedObject.GetComponent<MeshRenderer>()?.bounds;

        if (bounds.HasValue)
        {
            var prediction = lastRecognition.predictions.FirstOrDefault();

            Vector3 position = Vector3.zero;

            var detectedObjectPosition = CalculateCenterOfDetectedObject(prediction);

            var foundPosition = GetArRaycastLogic(out position, detectedObjectPosition);

            //var position = CalculateBoundingBoxPosition(bounds.Value, prediction.boundingBox);

            placedObject.transform.position = position;
        }
    }

    private Vector3 CalculateCenterOfDetectedObject(Prediction prediction)
    {
        var boundingBox = prediction.boundingBox;

        float centerFromLeft = (float)(boundingBox.left + (boundingBox.width / 2));

        float centerFromTop = (float)(boundingBox.top + (boundingBox.height / 2));

        return new Vector3(centerFromLeft, centerFromTop, .0f);
    }

    private Vector3 CalculateBoundingBoxPosition(Bounds b, BoundingBox boundingBox)
    {
        Debug.Log($"BB: left {boundingBox.left}, top {boundingBox.top}, width {boundingBox.width}, height {boundingBox.height}");

        double centerFromLeft = boundingBox.left + (boundingBox.width / 2);
        double centerFromTop = boundingBox.top + (boundingBox.height / 2);
        Debug.Log($"BB CenterFromLeft {centerFromLeft}, CenterFromTop {centerFromTop}");

        double quadWidth = b.size.normalized.x;
        double quadHeight = b.size.normalized.y;
        Debug.Log($"Quad Width {b.size.normalized.x}, Quad Height {b.size.normalized.y}");

        double normalisedPos_X = (quadWidth * centerFromLeft) - (quadWidth / 2);
        double normalisedPos_Y = (quadHeight * centerFromTop) - (quadHeight / 2);

        return new Vector3((float)normalisedPos_X, (float)normalisedPos_Y, 0);
    }

    private bool IsObjectRecognized(RecognitionResponse recognition)
    {
        if (recognition != null && recognition.predictions.Any())
        {
            var prediction = recognition.predictions.FirstOrDefault();

            if (prediction != null && prediction.probability > ProbabilityTreshold)
            {
                return true;
            }
        }

        return false;
    }

    bool GetArRaycastLogic(out Vector3 hitPosition, Vector3 detectedPosition)
    {

        // 1
        var hits = new List<ARRaycastHit>();

        // 2
        bool hasHit = raycastManager.Raycast(detectedPosition, hits, TrackableType.PlaneWithinInfinity);

        // 3
        if (hasHit == false || hits.Count == 0)
        {
            hitPosition = Vector3.zero;
            return false;
        }
        else
        {
            hitPosition = hits[0].pose.position;
            return true;
        }
    }
}
