using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	
    Camera camera;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float speedMultiplier = 4f;
    public float yAxisInput = 0;


    void Start() {
        camera = GameObject.Find("Sandbox/VisualizationCam").GetComponent<Camera>();
    }

    void Update()
    {
        // Get input for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Get the direction of the mouse pointer
        Vector3 mouseDirection = GetMouseDirection();

        if (Input.GetKey(KeyCode.LeftShift)) speedMultiplier = 4f;
        else speedMultiplier = 1f;

        if (Input.GetKey(KeyCode.Space)) {
            yAxisInput = 1;
        } else if (Input.GetKey(KeyCode.LeftControl)) {
            yAxisInput = -1;
        } else {
            yAxisInput = 0;
        }

        // Move the camera
        transform.Translate((mouseDirection * verticalInput + transform.right * horizontalInput) * moveSpeed * speedMultiplier * Time.deltaTime, Space.World);
        transform.Translate(transform.up * yAxisInput * moveSpeed * Time.deltaTime, Space.World);

        // Rotate the camera based on mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX);
    }

    Vector3 GetMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 mouseDirection = hit.point - transform.position;
            mouseDirection.y = 0; // Ensure no vertical component
            return mouseDirection.normalized;
        }

        return Vector3.forward; // Default forward direction if no hit
    }
}
