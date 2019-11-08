using Microsoft.Msagl.Core.Layout;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackageDiagram : Diagram
{
	public GameObject depends;
	private HashSet<Tuple<string, string>> edges;

	protected override void Awake()
	{
		base.Awake();
		edges = new HashSet<Tuple<string, string>>();
	}

	protected override IEnumerator Loader(JObject data)
	{
		foreach (JProperty pckg in data["modules"])
		{
			var node = AddNode();
			node.GetComponent<Clickable>().triggerAction.AddListener(client.GetComponent<IDEManager>().ShowInEditor);
			node.name = pckg.Name;
			var background = node.transform.Find("Background");
			var header = background.Find("Header");
			var qpIndex = pckg.Name.LastIndexOf('.');
			var qp = qpIndex != -1 ? $"{pckg.Name.Substring(0, qpIndex)}\n" : "";
			header.GetComponent<TextMeshProUGUI>().text = $"<size=75%>{qp}</size>" + pckg.Value["name"].ToString();
			nodes.Add(pckg.Name, node);
			yield return new WaitForFixedUpdate();
		}
		foreach (JObject rel in data["relationships"])
		{
			var type = rel["type"].ToString();
			var prefab = edgePrefab;
			if (type == "depends")
			{
				prefab = depends;
				//no fallback here
				var fromId = rel["from"].ToString();
				var toId = rel["to"].ToString();
				if(fromId != toId)
				{
					var edgeKey = new Tuple<string, string>(fromId, toId);
					if (edges.Contains(edgeKey) == false)
					{
						AddEdge(nodes[fromId], nodes[toId], prefab);
						edges.Add(edgeKey);
					}
				}
			}
			yield return new WaitForFixedUpdate();
		}
		LoadFileInfo(data["fileMap"]);
		yield return new WaitForFixedUpdate();
		Layout();
	}

}
