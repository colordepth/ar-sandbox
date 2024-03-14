using System;
using UnityEngine;
using Windows.Kinect;

/// <summary>
/// AR Sandbox's interface to Xbox Kinect.
/// </summary>
public class KinectInterface : MonoBehaviour
{
    private KinectSensor kinect;
    private DepthFrameReader depthReader;
    private InfraredFrameReader infraredReader;

    [HideInInspector]
    public ushort[] DepthData;
    [HideInInspector]
    public ushort[] InfraredData;

    public int FrameWidth { get; private set; }

    public int FrameHeight { get; private set; }

    public static double GetMedian(ushort[] sourceNumbers)
    {
        //Framework 2.0 version of this method. there is an easier way in F4        
        if (sourceNumbers == null || sourceNumbers.Length == 0)
            throw new System.Exception("Median of empty array not defined.");

        //make sure the list is sorted, but use a new array
        ushort[] sortedPNumbers = (ushort[])sourceNumbers.Clone();
        Array.Sort(sortedPNumbers);

        //get the median
        int size = sortedPNumbers.Length;
        int mid = size / 2;
        double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
        return median;
    }

    /// <summary>
    /// Polls the sensor for the next depth frame.
    /// </summary>
    /// <returns>Whether new data is available.</returns>
    public bool PollDepth()
    {
        if (depthReader != null)
        {
            DepthFrame depthframe = depthReader.AcquireLatestFrame();

            if (depthframe != null)
            {
                depthframe.CopyFrameDataToArray(DepthData);
                depthframe.Dispose();
                // Debug.Log(GetMedian(DepthData));
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Polls the sensor for the next infrared frame.
    /// </summary>
    /// <returns>Whether new data is available.</returns>
    public bool PollInfrared()
    {
        if (infraredReader != null)
        {
            InfraredFrame infraredframe = infraredReader.AcquireLatestFrame();

            if (infraredframe != null)
            {
                infraredframe.CopyFrameDataToArray(InfraredData);
                infraredframe.Dispose();
                // Debug.Log(GetMedian(DepthData));
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Acquire kinect sensor and its depth sensor's resolution.
    /// </summary>
    void Start()
    {
        kinect = KinectSensor.GetDefault();

        if (kinect == null)
        {
            Debug.LogErrorFormat("KinectInterface: Kinect sensor not found!");
            return;
        }

        if (!kinect.IsOpen)
            kinect.Open();

        depthReader = kinect.DepthFrameSource.OpenReader();

        FrameDescription depthFrameAttribs = depthReader.DepthFrameSource.FrameDescription;
        this.FrameWidth = depthFrameAttribs.Width;
        this.FrameHeight = depthFrameAttribs.Height;

        Debug.Log(depthFrameAttribs.HorizontalFieldOfView);
        Debug.Log(depthFrameAttribs.VerticalFieldOfView);
        Debug.Log(depthFrameAttribs.DiagonalFieldOfView);
        Debug.Log(depthFrameAttribs.BytesPerPixel);

        // Allocate memory for depth and infrared image
        this.DepthData = new ushort[depthFrameAttribs.LengthInPixels];
        this.InfraredData = new ushort[depthFrameAttribs.LengthInPixels];

        print("Kinect found!");
    }

    void Destroy()
    {
        Debug.Log("Destroying KinectInterface");

        if (depthReader != null)
        {
            depthReader.Dispose();
            depthReader = null;

            infraredReader.Dispose();
            infraredReader = null;
        }

        if (kinect != null)
        {
            if (kinect.IsOpen)
                kinect.Close();

            kinect = null;
        }
    }
}
