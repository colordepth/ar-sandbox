using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
	public int id;
	public Vector3 location;
	public int size;
	public Color32 color;
	public enum Type {
		CIRCLE,
		SQUARE
	};
	public Type type;

	public Point (int id, Vector3 location, int size, Color32 color, Type type) {
		this.id = id;
		this.location = location;
		this.size = size;
		this.color = color;
		this.type = type;
	}
}
