using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Polygon {
	public int id;
	public Vector3[] handles;
	public int size;
	public Color32 color;
	public enum Type
	{
		SOLID
	}
	public Type type;
	
	public Polygon (int id, Vector3[] handles, int size, Color32 color, Type type) {
		this.id = id;
		this.handles = handles;
		this.size = size;
		this.color = color;
		this.type = type;
	}
}
