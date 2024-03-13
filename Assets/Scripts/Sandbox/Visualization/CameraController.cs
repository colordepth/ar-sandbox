using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    void Update()
    {
        // Get input for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Get the direction of the mouse pointer
        Vector3 mouseDirection = GetMouseDirection();

        // Move the camera
        transform.Translate((mouseDirection * verticalInput + transform.right * horizontalInput) * moveSpeed * Time.deltaTime, Space.World);

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
