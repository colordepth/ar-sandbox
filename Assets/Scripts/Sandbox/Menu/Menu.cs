using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	public enum Page
	{
		HOME,			// controls, title
		ASSIST,		// terrain formation assistance
		DEPTH,		// depth map
		SATELLITE,	// satellite image
	}
	public Page page; 
	public int isDrawing;
	public int isVisualizing;

	GameObject sandbox;
	GameObject camera;
	GameObject visualizationCamera;

	void Start () {
		page = Page.HOME;
		isDrawing = 0;
		isVisualizing = 0;
		camera = GameObject.Find("Sandbox/Camera");
		visualizationCamera = GameObject.Find("Sandbox/VisualizationCam");
		sandbox = GameObject.Find("Sandbox");
		visualizationCamera.GetComponent<Camera>().enabled = false;
	}
	
	void Update () {
		getInput();
		pageController();
	}

	void getInput() {
		if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Escape)){
			page = Page.HOME;
		}
		if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
			page = Page.ASSIST;
		if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
			page = Page.DEPTH;
		if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
			page = Page.SATELLITE;
		if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)){
			if (isDrawing == 0) isDrawing = 1;
			else isDrawing = 0;
		}
		if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)){
			if (isVisualizing == 0) {
				isVisualizing = 1;
				sandbox.GetComponent<Sandbox>().mapMovement = 0;
				visualizationCamera.GetComponent<Camera>().enabled = true;
				camera.GetComponent<Camera>().enabled = false;
			}
			else {
				isVisualizing = 0;
				visualizationCamera.GetComponent<Camera>().enabled = false;
				camera.GetComponent<Camera>().enabled = true;
			}
		}
	}

	void pageController() {
		switch (page)
		{
			case Page.HOME:
				camera.transform.position = new Vector3(-500f, 500f, -235f);	
				isVisualizing = 0;
				visualizationCamera.GetComponent<Camera>().enabled = false;
				camera.GetComponent<Camera>().enabled = true;
				break;		
			case Page.ASSIST:
				camera.transform.position = new Vector3(6f, 500f, -235f);	
				break;		
			case Page.DEPTH:
				camera.transform.position = new Vector3(6f, 500f, -235f);	
				break;
			default:
				camera.transform.position = new Vector3(6f, 500f, -235f);	
				break;		
		}
	}
}
