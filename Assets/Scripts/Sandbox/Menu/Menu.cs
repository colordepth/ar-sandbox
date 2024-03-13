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

	public enum Mode {
		VIEW,
		DRAW,
		// VR_VISUALIZATION, 	// visualization in VR
		// SIMULATION,		// simulation mode in PC
	}

	public Page page; 
	public Mode mode;
	public int isVisualizing;
	GameObject camera;
	GameObject visualizationCamera;

	void Start () {
		page = Page.HOME;
		mode = Mode.VIEW;
		isVisualizing = 0;
		camera = GameObject.Find("Sandbox/Camera");
		visualizationCamera = GameObject.Find("Sandbox/VisualizationCam");
		visualizationCamera.GetComponent<Camera>().enabled = false;
	}
	
	void Update () {
		getInput();
		pageController();
	}

	void getInput() {
		if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Escape))
			page = Page.HOME;
		if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
			page = Page.ASSIST;
		if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
			page = Page.DEPTH;
		if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
			page = Page.SATELLITE;
		if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)){
			if (mode == Mode.DRAW) mode = Mode.VIEW;
			else if (mode == Mode.VIEW) mode = Mode.DRAW;
		}
		if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6)){
			if (isVisualizing == 0) {
				isVisualizing = 1;
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
