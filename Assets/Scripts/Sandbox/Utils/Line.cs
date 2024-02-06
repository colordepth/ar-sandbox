using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Line {
	public Vector3[] handles;
	public int size;
	public Color32 color;
	public string type;
	
	public Line (Vector3[] handles, int size, Color32 color, string type) {
		this.handles = new Vector3[handles.Length];
		Array.Copy(handles, this.handles, handles.Length);
		this.size = size;
		this.color = color;
		this.type = type;
	}
}
