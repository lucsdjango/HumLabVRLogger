using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Varjo.XR;
using Object = UnityEngine.Object;

// Heavily based on the EyeTrackingExample in Varjo SDK 
public class ETLogger : MonoBehaviour
{ 
    [Header("Gaze calibration settings")]
    [Tooltip("Legacy is the longer calib. procedure")]
    public VarjoEyeTracking.GazeCalibrationMode gazeCalibrationMode = VarjoEyeTracking.GazeCalibrationMode.Legacy;
    public KeyCode calibrationRequestKey = KeyCode.C;

    [Header("Gaze output filter settings")]
    public VarjoEyeTracking.GazeOutputFilterType gazeOutputFilterType = VarjoEyeTracking.GazeOutputFilterType.None; 

    [Header("Gaze data output frequency")]
    public VarjoEyeTracking.GazeOutputFrequency frequency;

    [Header("Toggle fixation point indicator visibility")]
    public bool showFixationPoint = false;
    [Header("Toggle closest hit point indicator visibility")]
    public bool showClosestHitPoint = false;


    [Header("Visualization Transforms")]
    public Transform fixationPointTransform, closestHitTransform;

    [Header("XR camera")]
    public Camera xrCamera;

    [Header("Gaze ray radius")]
    public float gazeRadius = 0.01f;

    [Header("Gaze point distance if no hit")]
    public float floatingGazeTargetDistance = 25f;

    // To log ET capture rate.
    private int gazeDataCount = 0;
    private float gazeTimer = 0f;

    public Logger logger;

    private VarjoEyeTracking.GazeData gazeData;
    private List<VarjoEyeTracking.GazeData> dataSinceLastUpdate;
    private List<VarjoEyeTracking.EyeMeasurements> eyeMeasurementsSinceLastUpdate;

    private Vector3 direction;
    private Vector3 rayOrigin;
    private RaycastHit hit;
    private float distance;
    private StreamWriter writer = null;

    // Fields in VarjoEyeTracking.GazeData 
    private static readonly string[] ColumnNames = { "Frame", "CaptureTime", "LogTime", "HMDPosition", "HMDRotation", "GazeStatus", "CombinedGazeForward", "CombinedGazePosition", "InterPupillaryDistanceInMM", "LeftEyeStatus", "LeftEyeForward", "LeftEyePosition", "LeftPupilIrisDiameterRatio", "LeftPupilDiameterInMM", "RightEyeStatus", "RightEyeForward", "RightEyePosition", "RightPupilIrisDiameterRatio", "RightPupilDiameterInMM", "FocusDistance", "FocusStability" };

    public string logFilePrefix;

    // Start is called before the first frame update
    void Start()
    {

        foreach (string label in ColumnNames)
            logger.AddEntry(label);

        foreach(GazeGrabLoggedObject gglo in UnityEngine.Object.FindObjectsByType(typeof(GazeGrabLoggedObject), FindObjectsSortMode.None))
        {
            if(gglo.logGrabbed)
                logger.AddEntry(gglo.name + "Grabbed", "0");
            if (gglo.logGazed)
                logger.AddEntry(gglo.name + "Gazed", "0", true);
            if(gglo.logPos)
                logger.AddEntry(gglo.name + "Pos", "");

        }
        
        logger.AddEntry("focusPoint", "-", true);
        logger.AddEntry("closestHitPoint", "-", true);


        // For testing: logging is started by calling StartLogging()
         Invoke("StartLogging", 2.0f);


    }

    private void StartLogging()
    {
        int dummy = VarjoEyeTracking.GetGazeList(out dataSinceLastUpdate, out eyeMeasurementsSinceLastUpdate);
        logger.StartLogging(logFilePrefix);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        fixationPointTransform.GetComponent<Renderer>().enabled = showFixationPoint;
        closestHitTransform.GetComponent<Renderer>().enabled = showClosestHitPoint;

        // Request gaze calibration
        if (Input.GetKeyDown(calibrationRequestKey))
        {
            VarjoEyeTracking.RequestGazeCalibration(gazeCalibrationMode);
        }


        //should be after gaze is logged
        if (Input.GetKeyDown(KeyCode.Space))
        {
            logger.UpdateEntry("testLogVariable1", Time.time.ToString());
            logger.UpdateEntry("testLogVariable2", Time.time.ToString());
        }


        foreach (GazeGrabLoggedObject gglo in UnityEngine.Object.FindObjectsByType(typeof(GazeGrabLoggedObject), FindObjectsSortMode.None))
        {
            if(gglo.logGrabbed)
                logger.UpdateEntry(gglo.name + "Grabbed", gglo.IsGrabbed() ? "1" : "0");
            if(gglo.logPos)
                logger.UpdateEntry(gglo.name + "Pos", gglo.transform.position.ToString("F4"));
        }

        // Get gaze data if gaze is allowed and calibrated
        if (VarjoEyeTracking.IsGazeAllowed() && VarjoEyeTracking.IsGazeCalibrated())
        {
            gazeData = VarjoEyeTracking.GetGaze();

            if (gazeData.status != VarjoEyeTracking.GazeStatus.Invalid)
            {

                /*
                // GazeRay vectors are relative to the HMD pose so they need to be transformed to world space
                if (gazeData.leftStatus != VarjoEyeTracking.GazeEyeStatus.Invalid)
                {
                    leftEyeTransform.position = xrCamera.transform.TransformPoint(gazeData.left.origin);
                    leftEyeTransform.rotation = Quaternion.LookRotation(xrCamera.transform.TransformDirection(gazeData.left.forward));
                }

                if (gazeData.rightStatus != VarjoEyeTracking.GazeEyeStatus.Invalid)
                {
                    rightEyeTransform.position = xrCamera.transform.TransformPoint(gazeData.right.origin);
                    rightEyeTransform.rotation = Quaternion.LookRotation(xrCamera.transform.TransformDirection(gazeData.right.forward));
                }
                */
                // Set gaze origin as raycast origin
                rayOrigin = xrCamera.transform.TransformPoint(gazeData.gaze.origin);

                // Set gaze direction as raycast direction
                direction = xrCamera.transform.TransformDirection(gazeData.gaze.forward);

                // Fixation point can be calculated using ray origin, direction and focus distance
                fixationPointTransform.position = rayOrigin + direction * gazeData.focusDistance;

                distance = floatingGazeTargetDistance;
                GazeGrabLoggedObject hitObject;
                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, direction, floatingGazeTargetDistance);



                if (hits.Length > 0)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if( hitObject = hit.transform.GetComponent<GazeGrabLoggedObject>())  { 
                            distance = Mathf.Min(distance, hit.distance);
                            if (hitObject.logGazed)
                                logger.UpdateEntry(hit.transform.name + "Gazed", "1");
                        }
                    }

                }

            }

            closestHitTransform.position = rayOrigin + direction * distance;

            if (logger.logging)
            {
                gazeTimer += Time.deltaTime;
                if (gazeTimer >= 1.0f)
                {
                    gazeDataCount = 0;
                    gazeTimer = 0f;
                
                }

                logger.UpdateEntry("focusPoint", fixationPointTransform.position.ToString("F4"));
                logger.UpdateEntry("closestHitPoint", closestHitTransform.position.ToString("F4"));
                int dataCount = VarjoEyeTracking.GetGazeList(out dataSinceLastUpdate, out eyeMeasurementsSinceLastUpdate);
                gazeDataCount += dataCount;
                for (int i = 0; i < dataCount; i++)
                {
                    LogGazeData(dataSinceLastUpdate[i], eyeMeasurementsSinceLastUpdate[i], i==(dataCount-1));
                }
            }




        }


    }

    void LogGazeData(VarjoEyeTracking.GazeData data, VarjoEyeTracking.EyeMeasurements eyeMeasurements, bool lastInList)
    {

        logger.UpdateEntry("Frame", data.frameNumber.ToString());
        logger.UpdateEntry("CaptureTime", data.captureTime.ToString());
        logger.UpdateEntry("LogTime", (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString());

        //logg difference!

        logger.UpdateEntry("HMDPosition", xrCamera.transform.localPosition.ToString("F3"));
        logger.UpdateEntry("HMDRotation", xrCamera.transform.localRotation.ToString("F3"));

        bool invalid = data.status == VarjoEyeTracking.GazeStatus.Invalid;
        logger.UpdateEntry("GazeStatus", invalid ? "INVALID" : "VALID");
        logger.UpdateEntry("CombinedGazeForward", invalid ? "" : data.gaze.forward.ToString("F3"));
        logger.UpdateEntry("CombinedGazePosition", invalid ? "" : data.gaze.origin.ToString("F3"));

        // IPD
        logger.UpdateEntry("InterPupillaryDistanceInMM", invalid ? "" : eyeMeasurements.interPupillaryDistanceInMM.ToString("F3"));

        
        // Left eye
        bool leftInvalid = data.leftStatus == VarjoEyeTracking.GazeEyeStatus.Invalid;
        logger.UpdateEntry("LeftEyeStatus", leftInvalid ? "INVALID" : "VALID");

        if (!leftInvalid)
        {
            logger.UpdateEntry("LeftEyeForward", data.left.forward.ToString("F3"));
            logger.UpdateEntry("LeftEyePosition", data.left.origin.ToString("F3"));
            logger.UpdateEntry("LeftPupilIrisDiameterRatio", eyeMeasurements.leftPupilIrisDiameterRatio.ToString("F3"));
            logger.UpdateEntry("LeftPupilDiameterInMM", eyeMeasurements.leftIrisDiameterInMM.ToString("F3"));
        }

        // Right eye
        bool rightInvalid = data.rightStatus == VarjoEyeTracking.GazeEyeStatus.Invalid;
        logger.UpdateEntry("RightEyeStatus", rightInvalid ? "INVALID" : "VALID");

        if (!rightInvalid)
        {
            logger.UpdateEntry("RightEyeForward", data.right.forward.ToString("F3"));
            logger.UpdateEntry("RightEyePosition", data.right.origin.ToString("F3"));
            logger.UpdateEntry("RightPupilIrisDiameterRatio", eyeMeasurements.rightPupilIrisDiameterRatio.ToString("F3"));
            logger.UpdateEntry("RightPupilDiameterInMM", eyeMeasurements.rightIrisDiameterInMM.ToString("F3"));
        }

        // Focus
        logger.UpdateEntry("FocusDistance", invalid ? "" : data.focusDistance.ToString());
        logger.UpdateEntry("FocusStability", invalid ? "" : data.focusStability.ToString());
        
        if (lastInList) 
            logger.Log();
        else
            logger.AsyncLog();
    }

}
