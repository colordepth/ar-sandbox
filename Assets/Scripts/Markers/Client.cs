
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

[Serializable]
public class JsonData
{
    public int[] label;
    public int[] x_pos;
    public int[] y_pos;
}
public class Client : MonoBehaviour
{
    private string serverURL = "http://localhost:3000/upload";
    private KinectInterface iKinect;

    public int[] xPos;
    public int[] yPos;
    public int[] labels;

    public enum MarkerType { Outlier_M, Circle_M, Square_M, RollingPin_M, Soldier_M, Devil_M, Plane_M, Tank_M, Fingertip_M }

    void Start()
    {
        iKinect = GetComponentInParent<Sandbox>().GetComponentInChildren<KinectInterface>();
        if (!iKinect)
        {
            Debug.Log("KinectInterface not found!");
            return;
        }

        StartCoroutine(SendImage());
    }

    IEnumerator SendImage()
    {
        while (true)
        {
            if (!iKinect)
            {
                print("Client: Kinect not found");
                yield return new WaitForSeconds(1);
                continue;
            }

            if (!iKinect.PollInfrared())
            {
                print("Client: Infrared empty");
                yield return new WaitForSeconds(1);
                continue;
            }

            yield return new WaitForSeconds(0.0333f);

            //print("Client: Initializing texture");

            Texture2D infraredTexture = new Texture2D(iKinect.InfraredFrameWidth, iKinect.InfraredFrameHeight, TextureFormat.RGB24, false);

            var pixels = new Color[iKinect.InfraredData.Length];

            for (var i = 0; i < pixels.Length; i++)
            {
                ushort intensity = iKinect.InfraredData[i];
                // Map the 16-bit ushort value (0 to 65535) into a normal 8-bit float value (0 to 1)
                var scale = (float)intensity / ushort.MaxValue;
                scale /= (0.08f * 3.0f);
                scale = Math.Min(1.0f, scale);
                scale = Math.Max(0.01f, scale);
                // Then simply use that scale value for all three channels R,G and B
                // => grey scaled pixel colors
                var pixel = new Color(scale, scale, scale);
                pixels[i] = pixel;

                //// Inspired from SDK Browser's Infrared D2D example

                //if (i == 3000) print("1, " + intensity.ToString());
                //float intensityRatio = (float)intensity / (float)ushort.MaxValue;
                //if (i == 3000) print("2, " + intensityRatio.ToString());

                //// dividing by the (average scene value * standard deviations)
                //intensityRatio /= (0.08f * 3.0f);
                //if (i == 3000) print("3, " + intensityRatio.ToString());
                //intensityRatio = Math.Min(1.0f, intensityRatio);
                //intensityRatio = Math.Max(0.01f, intensityRatio);

                //int pixelVal = (int)(intensityRatio * 255.0f);
                //var pixel = new Color(pixelVal, pixelVal, pixelVal);
                //pixels[i] = pixel;
            }

            //print("Client: Loading raw texture data");
            infraredTexture.SetPixels(pixels);

            //print("Client: Applying texture");
            infraredTexture.Apply();

            // Convert Texture2D to byte array
            //print("Client: Encoding to PNG");
            byte[] imageData = infraredTexture.EncodeToPNG();

            //print("Client: Creating WWWForm");
            // Create a form with image data
            WWWForm form = new WWWForm();
            form.AddBinaryData("image", imageData, "image.png", "image/png");

            // Send the POST request
            using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
            {
                //print("Client: Sending web request");
                yield return www.SendWebRequest();

                // Parse JSON response
                string jsonResponse = www.downloadHandler.text;
                //print(jsonResponse);

                JsonData data = JsonUtility.FromJson<JsonData>(jsonResponse);

                labels = data.label;
                xPos = data.x_pos;
                yPos = data.y_pos;


                for (int i = 0; i < xPos.Length; i++)
                {
                    int x_pos = xPos[i];
                    int y_pos = yPos[i];
                    int label = labels[i];

                    if (label == 0) continue;

                    print(x_pos + " _ " + y_pos + " _ " + label);
                }
            }
        }
    }
}