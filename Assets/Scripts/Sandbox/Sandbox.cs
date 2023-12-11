using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Sandbox : MonoBehaviour
{
    private new Camera camera;
    private TerrainMesh terrainmesh;
    string fileName = "CalibrationConfig.txt";
    StreamWriter stream;

    void Start ()
    {
        camera = GetComponentInChildren<Camera>();
        terrainmesh = GetComponentInChildren<KinectMesh>(false);

        if (terrainmesh == null)
            terrainmesh = GetComponentInChildren<DummyMesh>(false);
    }

    void WriteConfig()
    {
        if (File.Exists(fileName))
        {
            Debug.Log(fileName + " already exists.");
        }

        stream = File.CreateText(fileName);
        stream.WriteLine("Orthographic " + camera.orthographicSize);
        stream.WriteLine("Position " + terrainmesh.transform.position);
        stream.WriteLine("LocalScale " + terrainmesh.transform.localScale);
        stream.Close();
    }

    void Update ()
    {
        

        if (Input.GetKey(KeyCode.Q))
        {
            camera.orthographicSize++;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.E))
        {
            camera.orthographicSize--;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.W))
        {
            terrainmesh.transform.position += Vector3.forward * 100f * Time.smoothDeltaTime;\
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.S))
        {
            terrainmesh.transform.position += Vector3.back * 100f * Time.smoothDeltaTime;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.A))
        {
            terrainmesh.transform.position += Vector3.left * 100f * Time.smoothDeltaTime;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.D))
        {
            terrainmesh.transform.position += Vector3.right * 100f * Time.smoothDeltaTime;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            terrainmesh.transform.localScale += Vector3.up * 0.1f * Time.smoothDeltaTime;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            terrainmesh.transform.localScale += Vector3.down * 0.1f * Time.smoothDeltaTime;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            terrainmesh.transform.localScale += Vector3.left * 0.1f * Time.smoothDeltaTime;
            WriteConfig();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            terrainmesh.transform.localScale += Vector3.right * 0.1f * Time.smoothDeltaTime;
            WriteConfig();
        }

        // Lock bound range
        if (Input.GetKeyUp(KeyCode.L) && terrainmesh is KinectMesh)
        {
            ((KinectMesh)terrainmesh).lockBoundRange ^= true;
        }
    }
}
