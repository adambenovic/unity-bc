using UnityEngine;

public class Relationship
{
	public GameObject edge;

	public GameObject from;

	public GameObject to;

	public string type;

	public Relationship(GameObject edge, GameObject from, GameObject to, string type)
	{
		this.edge = edge;
		this.from = from;
		this.to = to;
		this.type = type;
	}
}
