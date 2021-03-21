using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSetter : MonoBehaviour
{
    public Text textDist;
    public Text heading;
    public Text realHeading;
    public Text log;
    // Update is called once per frame
    void Start()
    {
        Application.logMessageReceived += HandleLog;
        Debug.Log("Started Text setter");
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        var myLog = condition;
        string newString = "\n [" + type + "] : " + myLog;
        if(log.text.Length < 200)
        {
            log.text += "  " + newString;
        }
        else
        {
            log.text = newString;
        }
    }

    void Update()
    {
        bool Bheading = GPSService.Instance.IsLookingAtCoords();
        textDist.text = "D = " + GPSService.Instance.distance.ToString();
        realHeading.text =" Heading "+ GPSService.Instance.trueHeading.ToString();
        heading.text = "Looking at it? " + Bheading.ToString();
    }
}
