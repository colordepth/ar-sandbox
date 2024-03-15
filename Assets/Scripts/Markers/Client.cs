
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
    private int imageWidth = 640;
    private int imageHeight = 480;
    private int imageChannels = 3;
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
                yield break;

            if (!iKinect.PollInfrared())
                continue;

            //byte[] imageBytes = new byte[imageWidth * imageHeight];
            //Buffer.BlockCopy(iKinect.InfraredData, 0, imageBytes, 0, iKinect.InfraredData.Length);


            Texture2D texture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
            Color32[] pixels = new Color32[imageWidth * imageHeight];

            print("Compare: " + (imageWidth * imageHeight) + " " + iKinect.InfraredData.Length);

            for (int i = 0; i < pixels.Length; i++)
            {
                //byte colorVal = (byte)(iKinect.InfraredData[i]/255);
                //pixels[i] = new Color32(colorVal, colorVal, colorVal, 255);
            }
            texture.SetPixels32(pixels);
            texture.Apply();

            byte[] imageData = texture.EncodeToPNG();

            // Create a form with image data
            WWWForm form = new WWWForm();
            form.AddBinaryData("image", imageData, "image.png", "image/png");

            // Send the POST request
            using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
            {
                yield return www.SendWebRequest();

                // Parse JSON response
                string jsonResponse = www.downloadHandler.text;

                JsonData data = JsonUtility.FromJson<JsonData>(jsonResponse);

                labels = data.label;
                xPos = data.x_pos;
                yPos = data.y_pos;


                for (int i = 0; i < xPos.Length; i++)
                {
                    int x_pos = xPos[i];
                    int y_pos = yPos[i];
                    int label = labels[i];

                    print(x_pos + " _ " + y_pos + " _ " + label);
                }
            }
        }
    }
}