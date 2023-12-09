using UnityEngine;
using Windows.Kinect;

/// <summary>
/// AR Sandbox's interface to Xbox Kinect.
/// </summary>
public class KinectInterface : MonoBehaviour
{
	private KinectSensor kinect;
	private DepthFrameReader depthReader;

	[HideInInspector]
	public ushort[] DepthData;

    public int FrameWidth { get; private set; }

    public int FrameHeight { get; private set; }

    /// <summary>
    /// Polls the sensor for the next depth frame.
    /// </summary>
	/// <returns>Whether new data is available.</returns>
    private bool Poll()
	{
		if (depthReader != null)
		{
			DepthFrame depthframe = depthReader.AcquireLatestFrame();
			if (depthframe != null)
			{
				depthframe.CopyFrameDataToArray(DepthData);
                depthframe.Dispose();
				return true;
			}
		}
		return false;
	}

    /// <summary>
    /// Acquire kinect sensor and its depth sensor's resolution.
    /// </summary>
    void Start ()
	{
		kinect = KinectSensor.GetDefault();

		if (kinect == null)
		{
			Debug.LogErrorFormat("KinectInterface: Kinect sensor not found!");
			return;
		}

		depthReader = kinect.DepthFrameSource.OpenReader();

		FrameDescription depthFrameAttribs = depthReader.DepthFrameSource.FrameDescription;
		this.FrameWidth = depthFrameAttribs.Width;
		this.FrameHeight = depthFrameAttribs.Height;

		// Allocate memory for depth image
		this.DepthData = new ushort[depthFrameAttribs.LengthInPixels];
    }

	void Destroy()
	{
		if (depthReader != null)
		{
			depthReader.Dispose();
			depthReader = null;
		}

		if (kinect != null)
		{
			if (kinect.IsOpen)
				kinect.Close();

			kinect = null;
		}
	}
}
