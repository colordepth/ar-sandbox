using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : MonoBehaviour
{
    private new Camera camera;
    private TerrainMesh terrainmesh;

    void Start ()
    {
        camera = GetComponentInChildren<Camera>();
        terrainmesh = GetComponentInChildren<KinectMesh>(false);

        if (terrainmesh == null)
            terrainmesh = GetComponentInChildren<DummyMesh>(false);
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            camera.orthographicSize++;
        }

        if (Input.GetKey(KeyCode.E))
        {
            camera.orthographicSize--;
        }

        if (Input.GetKey(KeyCode.W))
        {
            terrainmesh.transform.position += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            terrainmesh.transform.position += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A))
        {
            terrainmesh.transform.position += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            terrainmesh.transform.position += Vector3.right;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            terrainmesh.transform.localScale += Vector3.up * 0.01f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            terrainmesh.transform.localScale += Vector3.down * 0.01f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            terrainmesh.transform.localScale += Vector3.left * 0.01f;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            terrainmesh.transform.localScale += Vector3.right * 0.01f;
        }
    }
}
