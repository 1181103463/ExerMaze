using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GPXMovementTracker : MonoBehaviour
{
    private double initialLatitude;
    private double initialLongitude;
    private float movementScale = 0.000009f; // Scale for Unity units to lat/lon, no duplicate
    private List<(double latitude, double longitude, float elevation, string timestamp)> characterTrackPoints 
        = new List<(double, double, float, string)>();

    private List<(double latitude, double longitude, float elevation, string timestamp)> realLifeTrackPoints 
        = new List<(double, double, float, string)>();
    private double characterLatitude;
    private double characterLongitude;

    private double realLifeLatitude;
    private double realLifeLongitude;
    private Vector3 lastPosition; // testing duplicate

    private int stepCount = 0; // For real-life tracking, no duplicate

    public double GetCurrentLatitude() => characterLatitude;
    public double GetCurrentLongitude() => characterLongitude;

    void Start()
    {
        if (BLEManager.Instance != null && BLEManager.Instance.bleDataHandler != null)
        {
            BLEManager.Instance.bleDataHandler.OnStepReceived += HandleStepReceived;
        }

        int lastIndex = PlayerPrefs.GetInt("SelectedCoordinateIndex", 0);
        lastIndex = Mathf.Clamp(lastIndex, 0, GPXCoordinate.GetSavedCoordinates().Count - 1);
        GPXCoordinate.SetInitialFromSaved(lastIndex);
        initialLatitude = GPXCoordinate.InitialLatitude;
        initialLongitude = GPXCoordinate.InitialLongitude;

        //ResetTracking(); // Always reset when the level starts
    }

    private void OnDestroy()
    {
        if (BLEManager.Instance != null && BLEManager.Instance.bleDataHandler != null)
        {
            BLEManager.Instance.bleDataHandler.OnStepReceived -= HandleStepReceived;
        }
    }

    public void ResetTracking()
    {
        Debug.Log("GPXMovementTracker: Resetting tracking data.");
        characterTrackPoints.Clear();
        realLifeTrackPoints.Clear();
        stepCount = 0; // Reset step count for real-life tracking

        initialLatitude = GPXCoordinate.InitialLatitude;
        initialLongitude = GPXCoordinate.InitialLongitude;
        characterLatitude = initialLatitude;
        characterLongitude = initialLongitude;

        realLifeLatitude = initialLatitude;
        realLifeLongitude = initialLongitude;

        lastPosition = transform.position;
        AddTrackPoint(characterTrackPoints, characterLatitude, characterLongitude);
        AddTrackPoint(realLifeTrackPoints, realLifeLatitude, realLifeLongitude);
    }

    private void HandleStepReceived()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.InGame &&
            FindAnyObjectByType<LevelManager>().CurrentLevelState == LevelManager.LevelState.Playing)
        {
            // if (GPXCoordinate.CurrentTrackingMode == GPXCoordinate.TrackingMode.CharacterTracking)
            // {
            //     TrackCharacterMovement();
            // }
            // else
            // {
            //     TrackRealLifeMovement();
            // }
            TrackCharacterMovement();
            TrackRealLifeMovement();
        }
    }

    private void TrackCharacterMovement()
    {
        Vector3 movement = transform.position - lastPosition;

        double deltaLatitude = movement.z * movementScale;
        double deltaLongitude = movement.x * (movementScale / Math.Cos(characterLatitude * (Math.PI / 180)));

        characterLatitude += deltaLatitude;
        characterLongitude += deltaLongitude;

        lastPosition = transform.position;

        AddTrackPoint(characterTrackPoints, characterLatitude, characterLongitude);
    }

    private void TrackRealLifeMovement()
    {
        float distance = GPXCoordinate.StepLength;
        float distanceInDegrees = distance / 111139f;

        float randomAngle = UnityEngine.Random.Range(0f, 360f);

        double deltaLatitude = distanceInDegrees * Math.Cos(randomAngle * (Math.PI / 180));
        double deltaLongitude = distanceInDegrees * Math.Sin(randomAngle * (Math.PI / 180)) 
                            / Math.Cos(realLifeLatitude * (Math.PI / 180));

        realLifeLatitude += deltaLatitude;
        realLifeLongitude += deltaLongitude;

        AddTrackPoint(realLifeTrackPoints, realLifeLatitude, realLifeLongitude);
    }

    private void AddTrackPoint(
        List<(double latitude, double longitude, float elevation, string timestamp)> list,
        double lat,
        double lon)
    {
        string timestamp = System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        float elevation = 0f;

        list.Add((lat, lon, elevation, timestamp));
    }

    public string GenerateCharacterGPXData()
    {
        return GenerateGPX(characterTrackPoints, "Character Movement");
    }

    public string GenerateRealLifeGPXData()
    {
        return GenerateGPX(realLifeTrackPoints, "Real-Life Movement");
    }

    private string GenerateGPX(
        List<(double latitude, double longitude, float elevation, string timestamp)> points,
        string trackName)
    {
        StringBuilder gpxData = new StringBuilder();

        gpxData.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        gpxData.AppendLine("<gpx version=\"1.1\" creator=\"GPXMovementTracker\" xmlns=\"http://www.topografix.com/GPX/1/1\">");
        gpxData.AppendLine("<trk>");
        gpxData.AppendLine($"<name>{trackName}</name>");
        gpxData.AppendLine("<trkseg>");

        foreach (var point in points)
        {
            gpxData.AppendLine($"<trkpt lat=\"{point.latitude}\" lon=\"{point.longitude}\">");
            gpxData.AppendLine($"  <ele>{point.elevation}</ele>");
            gpxData.AppendLine($"  <time>{point.timestamp}</time>");
            gpxData.AppendLine($"</trkpt>");
        }

        gpxData.AppendLine("</trkseg>");
        gpxData.AppendLine("</trk>");
        gpxData.AppendLine("</gpx>");

        return gpxData.ToString();
    }
}
