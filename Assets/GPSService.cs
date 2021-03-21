using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSService : MonoBehaviour
{
    public float lat = 0f;
    public float lon = 0f;
    
    public float trueHeading = 0f;
    
    public float destLat = 50.284456f;
    public float destLon = 18.966078f;

    // Distance in m between lat, lon and destLat, destLon
    public double distance = 1000;
    // Max radius in meters
    public double maxRadius = 50;

    public float maxAngle = 2f;
    

    public static GPSService Instance { get; set; }
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(ContinousGPSService());
    }
    private IEnumerator ContinousGPSService()
    {
        while (true)
        {
            //Debug.Log("Starting GPS query");
            StartCoroutine(StartLocationService());
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("User didn't enable GPS");
            yield break;
        }
        Input.location.Start();
        int maxWait = 20;
        while(maxWait > 0 && Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait <= 0)
        {
            Debug.LogError("GPS timed out");
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("GPS rq failed");
            yield break;
        }
        
        Input.compass.enabled = true;
        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;
        trueHeading = Input.compass.trueHeading;

        double latMid, m_per_deg_lat, m_per_deg_lon, deltaLat, deltaLon;
        latMid = (lat + destLat) / 2.0;  // or just use Lat1 for slightly less accurate estimate
        m_per_deg_lat = 111132.954 - 559.822 * Math.Cos(2.0 * latMid) + 1.175 * Math.Cos(4.0 * latMid);
        m_per_deg_lon = (3.14159265359 / 180) * 6367449 * Math.Cos(latMid);

        deltaLat = Math.Abs(lat - destLat);
        deltaLon = Math.Abs(lon - destLon);

        distance = Math.Sqrt(Math.Pow(deltaLat * m_per_deg_lat, 2) + Math.Pow(deltaLon * m_per_deg_lon, 2));
        Vector2 v = new Vector2(destLon - lon, destLat - lat);
        double angle = Vector2.SignedAngle(Vector2.up, v);
        if(angle < 0)
        {
            angle = 180 - angle;
        }

        trueHeading = (float)Math.Abs(angle - trueHeading);
        yield break;
    }

    public double GetDistance()
    {
        return distance;
    }

    public double GetHeading()
    {
        return trueHeading;
    }

    public bool IsLookingAtCoords()
    {
        return trueHeading < maxAngle || trueHeading > (360 - maxAngle);
    }
    public bool IsInVisibleRadius()
    {
        return distance < maxRadius;
    }
}
