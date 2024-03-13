using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker
{
	public int id;
	public int groupID; 
	public enum Type
	{
		POINT,
		LINE,
		POLYGON,
		FRIENDLY_SOLDIER,
		FRIENDLY_TANK,
		FRIENDLY_JET,
		HOSTILE_SOLDIER
	}

	public Vector3 location;
	public Type type;

	public Marker(int id, int groupID, Vector3 location, Type type)
	{
		this.id = id;
		this.groupID = groupID;
		this.location = location;
		this.type = type;
	}
}
