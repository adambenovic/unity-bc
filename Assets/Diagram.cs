using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Diagram : Graph
{

	public WSClient client;
	protected Dictionary<string, GameObject> nodes;

	public JObject FileMap
	{
		set; get;
	}

	protected override void Awake()
	{
		base.Awake();
		nodes = new Dictionary<string, GameObject>();
	}

	protected void LoadFileInfo(JToken fileMap)
	{
		foreach (JProperty prop in fileMap)
		{
			var filePath = prop.Name;
			foreach (JObject node in prop.Value)
			{
				string qname = node.Value<string>("qname");
				int lineFrom = node.Value<int>("fromLineNo");
				int lineTo = node.Value<int>("toLineNo");
				FileInfo fi = new FileInfo(prop.Name, lineFrom, lineTo);
				nodes[qname].GetComponent<UNode>().UserData = fi;
			}
		}
	}

	public void Load(JObject data)
	{
		FileMap = (JObject)data["fileMap"];
		StartCoroutine(Loader(data));
	}

	public GameObject Find(string qname)
	{
		return nodes.ContainsKey(qname) ? nodes[qname] : null;
	}

	protected abstract IEnumerator Loader(JObject data);

	public Dictionary<string, GameObject> AddNode(string name, GameObject node)
	{
		nodes.Add(name, node);

		return nodes;
	}
}
