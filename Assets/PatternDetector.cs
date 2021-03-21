using Assets;
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

    public GameObject placedObject;

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
    private Texture2D CaptureCameraTexture()
    {
        var cameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);

        cameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        return cameraTexture;
    }

    public void SearchForObject()
    {
        var cameraTexture = CaptureCameraTexture();

        if (lastRecognition == null) //TODO: w tej chwili wstawianie obiektu odbywa siê tylko raz, mo¿e warto co jakiœ czas usun¹æ stary obiekt i podmieniæ go na nowy?
        {
            var imageByteArray = GetImageAsByteArray(cameraTexture);

            StartCoroutine(MakePredictionRequest(imageByteArray));
        }
    }

    private IEnumerator MakePredictionRequest(byte[] byteData)
    {
        Debug.Log("Got into prediction method");
        WWWForm webForm = new WWWForm();
        using (UnityWebRequest request = UnityWebRequest.Post(Settings.ApiURL, webForm))
        {
            PrepareWebRequest(request, byteData);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                var responseMessage = request.downloadHandler.text;

                var recognition = GetRecognitionResponse(responseMessage);

                if (recognition != null && IsObjectRecognized(recognition))
                {
                    lastRecognition = recognition;

                    PlaceObjectInDetectedPlace();
                }
            }
        }
    }

    private void PrepareWebRequest(UnityWebRequest request, byte[] content)
    {
        request.SetRequestHeader("Content-Type", Settings.ContentType);

        request.SetRequestHeader("Prediction-Key", Settings.PredictionKey);

        request.uploadHandler = new UploadHandlerRaw(content);

        request.uploadHandler.contentType = Settings.ContentType;

        request.downloadHandler = new DownloadHandlerBuffer();
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

    private void PlaceObjectInDetectedPlace()
    {
        var prediction = lastRecognition.predictions.FirstOrDefault();

        Vector3 position = Vector3.zero;

        Quaternion rotation = new Quaternion();

        var detectedObjectPosition = CalculateCenterOfDetectedObject(prediction);

        var isPositionFound = GetArRaycastLogic(out position, out rotation, detectedObjectPosition);

        if (isPositionFound)
            placedObject = Instantiate(PrefabToPlace, position, rotation);
    }

    private Vector2 CalculateCenterOfDetectedObject(Prediction prediction)
    {
        var boundingBox = prediction.boundingBox;

        float centerFromLeft = (float)(boundingBox.left + (boundingBox.width / 2));

        float centerFromTop = (float)(boundingBox.top + (boundingBox.height / 2));

        var positionOnScreen = PlaceInScreenCoords(centerFromLeft, centerFromTop); //Z bounding boxa mo¿emy ustaliæ pozycje w zakresie 0-1, trzeba to przemno¿yæ przez faktyczne rozmiary ekranu

        return positionOnScreen;
    }

    private Vector2 PlaceInScreenCoords(float x, float y)
    {
        var newX = x * Screen.width;

        var newY = y * Screen.height;

        return new Vector2(newX, newY);
    }

    private bool IsObjectRecognized(RecognitionResponse recognition)
    {
        if (recognition != null && recognition.predictions.Any())
        {
            var prediction = recognition.predictions.FirstOrDefault();

            if (prediction != null && prediction.probability > ProbabilityTreshold)
                return true;
        }

        return false;
    }

    bool GetArRaycastLogic(out Vector3 hitPosition, out Quaternion rotation, Vector2 pos)
    {
        var hits = new List<ARRaycastHit>();

        bool hasHit = raycastManager.Raycast(pos, hits, TrackableType.PlaneWithinPolygon);

        if (hasHit == false || hits.Count == 0)
        {
            hitPosition = Vector3.zero;
            rotation = new Quaternion();
            return false;
        }
        else
        {
            hitPosition = hits[0].pose.position;
            rotation = hits[0].pose.rotation;
            return true;
        }
    }
}
