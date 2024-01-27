using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
	public Vector3 location;
	public int size;
	public Color32 color;
	public string type;

	public Point (Vector3 location, int size, Color32 color, string type) {
		this.location = location;
		this.size = size;
		this.color = color;
		this.type = type;
	}
}
