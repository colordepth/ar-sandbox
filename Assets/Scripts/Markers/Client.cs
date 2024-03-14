
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    private string serverURL = "http://localhost:3000/upload";
    private int imageWidth = 640;
    private int imageHeight = 480;
    private int imageChannels = 3;
    private KinectInterface iKinect;

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

            iKinect.PollInfrared();

            Texture2D texture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);

            byte[] imageBytes = new byte[imageWidth * imageHeight];
            Buffer.BlockCopy(iKinect.InfraredData, 0, imageBytes, 0, iKinect.InfraredData.Length);
            texture.LoadImage(imageBytes);

            // Convert Texture2D to byte array
            byte[] imageData = texture.EncodeToPNG();

            // Create a form with image data
            WWWForm form = new WWWForm();
            form.AddBinaryData("image", imageData, "image.png", "image/png");

            // Send the POST request
            using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
            {
                //yield return www.SendWebRequest();
                www.SendWebRequest();

                // Parse JSON response
                string jsonResponse = www.downloadHandler.text;

                print(jsonResponse);

                yield return jsonResponse;
                //List<Dictionary<string, float>> resultList = JsonUtility.FromJson<List<Dictionary<string, float>>>(jsonResponse);

                //// Access age and weight values from the resultList
                //foreach (var result in resultList)
                //{
                //    float age = result["age"];
                //    float weight = result["weight"];

                //    // Do something with age and weight values
                //    Debug.Log("Age: " + age + ", Weight: " + weight);
                //}
            }
        }
    }
}