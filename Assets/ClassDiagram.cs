using Microsoft.Msagl.Core.Layout;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class ClassDiagram : Diagram
{
	public GameObject association;
	public GameObject specialization;
	public GameObject implements;
	
	protected override void Awake()
	{
		base.Awake();
	}

	protected override IEnumerator Loader(JObject data)
	{
		foreach (JProperty cls in data["classes"])
		{
			var node = AddNode();
			node.GetComponent<Clickable>().triggerAction.AddListener(client.GetComponent<IDEManager>().ShowInEditor);
			node.GetComponent<Clickable>().triggerCtrlAction.AddListener(client.GetComponent<IDEManager>().Manage);
			node.name = cls.Name;
			var background = node.transform.Find("Background");
			var header = background.Find("Header");
			var attributes = background.Find("Attributes");
			var methods = background.Find("Methods");
			var type = cls.Value["type"].ToString();
			var stereotype = type != "class" ? $"<<{type}>>\n" : "";
			var qpIndex = cls.Name.LastIndexOf('.');
			var qp = qpIndex != -1 ? $"{cls.Name.Substring(0, qpIndex)}\n" : "";
			header.GetComponent<TextMeshProUGUI>().text = $"<size=75%>{stereotype}{qp}</size>" + cls.Value["name"].ToString();
			foreach (JProperty attr in cls.Value["attributes"])
			{
				attributes.GetComponent<TextMeshProUGUI>().text += attr.Name + "\n";
			}
			foreach (JProperty method in cls.Value["methods"])
			{
				methods.GetComponent<TextMeshProUGUI>().text += $"{method.Name}({String.Join(", ", method.Value["argnames"].Values<string>())})\n";
			}
			nodes.Add(cls.Name, node);
			yield return new WaitForFixedUpdate();
		}
		foreach (JObject rel in data["relationships"])
		{
			var type = rel["type"].ToString();
			var prefab = edgePrefab;
			if (type == "association")
			{
				prefab = association;
			}
			else if(type == "implements")
			{
				prefab = implements;
			}
			else if (type == "specialization")
			{
				prefab = specialization;
			}
			AddEdge(nodes[rel["from"].ToString()], nodes[rel["to"].ToString()], prefab);
			yield return new WaitForFixedUpdate();
		}
		LoadFileInfo(data["fileMap"]);
		yield return new WaitForFixedUpdate();
		Layout();
	}

}
