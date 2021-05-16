using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using SFB;
using TMPro;
using WebSocketSharp;

public class GridManager : MonoBehaviour
{
	private Transform gridUnits;
	public WSClient client;
	public GameObject buttonPrefab;
	public GameObject classDiagramPrefab;
	private Coroutine clickRoutine;
	private bool clickRoutineRunning;

	private ClassDiagram diagram;
	private Dictionary<string, RawClass> rawClassesFromFile = new Dictionary<string, RawClass>();
	private Dictionary<string, GameObject> classesFromFile = new Dictionary<string, GameObject>();
	private Dictionary<string, GameObject> secondaryClassesFromFile = new Dictionary<string, GameObject>();
	private Dictionary<string, GameObject> addedClasses = new Dictionary<string, GameObject>();

	private static readonly ExtensionFilter[] JsonExtension = new ExtensionFilter[]
	{
		new ExtensionFilter("JSON", "json"),
	};

	public void GridGenerate(ClassDiagram diagram)
	{
		this.diagram = diagram;
		gridUnits = transform.Find("GridUnits");
		var graphPosition = diagram.GetComponent<RectTransform>().rect;
		var xStart = graphPosition.xMin + 35;
		var yStart = graphPosition.yMax - 30;

		GameObject plus = Instantiate(buttonPrefab, gridUnits);
		plus.GetComponent<Clickable>().triggerAction.AddListener(plusAction);
		addTextToButton(plus,  "+");
		plus.transform.position = new Vector3(xStart, yStart, 25);

		GameObject minus = Instantiate(buttonPrefab, gridUnits);
		minus.GetComponent<Clickable>().triggerAction.AddListener(minusAction);
		addTextToButton(minus,  "-");
		minus.transform.position = new Vector3(xStart + 2.5f, yStart, 25);

		GameObject open = Instantiate(buttonPrefab, gridUnits);
		open.GetComponent<Clickable>().triggerAction.AddListener(OpenFile);
		addTextToButton(open,  "Open file");
		open.transform.position = new Vector3(xStart + 10, yStart, 25);

		GameObject export = Instantiate(buttonPrefab, gridUnits);
		export.GetComponent<Clickable>().triggerAction.AddListener(Export);
		addTextToButton(export,  "Export");
		export.transform.position = new Vector3(xStart + 20, yStart, 25);
	}

	private void addTextToButton(GameObject button, string text)
	{
		var background = button.transform.Find("Background");
		var header = background.Find("Header");
		header.GetComponent<TextMeshProUGUI>().text = text;
	}

	public void OpenFile(GameObject go)
	{
		var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "./Examples/", JsonExtension, false);

		if (paths.Length > 0)
		{
			rawClassesFromFile.Clear();
			classesFromFile.Clear();
			secondaryClassesFromFile.Clear();
			rawClassesFromFile = SerializationManager.LoadElementsFromFile(paths[0]);

			if (rawClassesFromFile == null || rawClassesFromFile.Count < 1)
			{
				return;
			}

			Regex reg = new Regex(@".*\..*\.(.*)");

			var oldGraph = gridUnits.Find("ClassDiagram(Clone)");
			if (oldGraph != null)
			{
				Destroy(oldGraph.gameObject);
			}

			var cdGo = Instantiate(classDiagramPrefab, gridUnits);
			var graphPos = diagram.GetComponent<RectTransform>().rect;
			cdGo.transform.position = new Vector3(graphPos.center.x + 25, graphPos.center.y + 5, 25);
			var classDiagram = cdGo.GetComponent<ClassDiagram>();

			foreach (var classObject in rawClassesFromFile)
			{
				var node = classDiagram.AddNode();
				node.name = classObject.Key;
				var background = node.transform.Find("Background");
				var header = background.Find("Header");
				var methods = background.Find("Methods");
				var type = "class";
				var stereotype = type != "class" ? $"<<{type}>>\n" : "";
				var qpIndex = classObject.Key.LastIndexOf('.');
				var qp = qpIndex != -1 ? $"{classObject.Key.Substring(0, qpIndex)}\n" : "";
				header.GetComponent<TextMeshProUGUI>().text = $"<size=75%>{stereotype}{qp}</size>" + classObject.Key;

				foreach (var method in classObject.Value.methods)
				{
					methods.GetComponent<TextMeshProUGUI>().text += $"{method}()\n";
				}

				classDiagram.AddNode(node.name, node);
				secondaryClassesFromFile.Add(node.name, node);

				foreach (var association in classObject.Value.associations)
				{
					foreach (var theNode in classDiagram.Nodes)
					{
						if (theNode.Key.Contains(association) && !theNode.Key.Equals(node.name))
						{
							classDiagram.AddEdge(theNode.Value, node, classDiagram.association);
						}
					}
				}
			}

			classDiagram.Layout();

			foreach (var classObject in diagram.Nodes)
			{
				var match = reg.Match(classObject.Key);
				string name = match.Groups[1].Value;
				if (name.IsNullOrEmpty())
				{
					name = classObject.Key;
				}

				if (rawClassesFromFile.ContainsKey(name))
				{
					classesFromFile.Add(classObject.Key, classObject.Value);
					rawClassesFromFile.Remove(name);
				}
			}
		}
	}

	public void plusAction(GameObject go)
	{
		List<string> toBeRemoved = new List<string>();
		foreach (var classObject in rawClassesFromFile)
		{
			var node = diagram.AddNode();
			node.GetComponent<Clickable>().triggerAction.AddListener(client.GetComponent<IDEManager>().Select);
			node.GetComponent<Clickable>().triggerCtrlAction.AddListener(client.GetComponent<IDEManager>().SelectMultiple);
			node.name = classObject.Key;
			var background = node.transform.Find("Background");
			var header = background.Find("Header");
			var methods = background.Find("Methods");
			var type = "class";
			var stereotype = type != "class" ? $"<<{type}>>\n" : "";
			var qpIndex = classObject.Key.LastIndexOf('.');
			var qp = qpIndex != -1 ? $"{classObject.Key.Substring(0, qpIndex)}\n" : "";
			header.GetComponent<TextMeshProUGUI>().text = $"<size=75%>{stereotype}{qp}</size>" + classObject.Key;

			foreach (var method in classObject.Value.methods)
			{
				methods.GetComponent<TextMeshProUGUI>().text += $"{method}()\n";
			}

			diagram.AddNode(node.name, node);
			node.GetComponent<BackgroundHighlighter>().HighlightOutline(Color.green);
			classesFromFile.Add(node.name, node);
			addedClasses.Add(node.name, node);
			toBeRemoved.Add(node.name);

			foreach (var association in classObject.Value.associations)
			{
				foreach (var theNode in diagram.Nodes)
				{
					if (theNode.Key.Contains(association))
					{
						GameObject edge = diagram.AddEdge(theNode.Value, node, diagram.association);
						diagram.Relationshipis.Add(new Relationship(edge, theNode.Value, node, "association"));
					}
				}
			}
		}

		foreach (var toRemove in toBeRemoved)
		{
			rawClassesFromFile.Remove(toRemove);
		}

		foreach (var classObject in classesFromFile)
		{
			diagram.AddNode(classObject.Key, classObject.Value);
		}

		foreach (var classObject in classesFromFile)
		{
			var prefab = diagram.association;
			string path = "";
			path = getFilePathFromNode(diagram, classObject.Key, path);
			FileInfo fi = new FileInfo(path, 0, 0);

			foreach (var relationship in diagram.Relationshipis)
			{
				if (relationship.from.name == classObject.Value.name && !relationship.edge.activeSelf)
				{
					if (relationship.type == "association")
					{
						prefab = diagram.association;
					}
					else if (relationship.type == "specialization")
					{
						prefab = diagram.specialization;
					}
					else if (relationship.type == "implements")
					{
						prefab = diagram.implements;
					}

					relationship.edge = diagram.AddEdge(relationship.from, relationship.to, prefab);
				}
				else if (fi.filePath.IsNullOrEmpty() && (relationship.from.name == classObject.Value.name ||
				                                         relationship.to.name == classObject.Value.name))
				{
					string fromPath = getFilePathFromNode(diagram, relationship.from.name, path);
					string toPath = getFilePathFromNode(diagram, relationship.to.name, path);
					if (fromPath != path)
					{
						path = fromPath;
					}
					else if (toPath != path)
					{
						path = toPath;
					}
				}
			}

			fi.filePath = path;
			diagram.Nodes[classObject.Key].GetComponent<UNode>().UserData = fi;
		}

		diagram.Layout();
	}

	private string getFilePathFromNode(ClassDiagram diagram, string key, string path)
	{
		try
		{
			FileInfo fi = (FileInfo) diagram.Nodes[key].GetComponent<UNode>().UserData;
			if (!fi.filePath.IsNullOrEmpty())
			{
				path = fi.filePath;
			}
		}
		catch (Exception e)
		{
		}

		return path;
	}

	public void minusAction(GameObject go)
	{
		List<GameObject> removedNodes = new List<GameObject>();
		List<GameObject> removedEdges = new List<GameObject>();
		var classesToRemove = client.GetComponent<IDEManager>().GetSelected();

		foreach (var classObject in classesToRemove.GetObjects())
		{
			foreach (var relationship in diagram.Relationshipis)
			{
				if (relationship.from.name == classObject.name || relationship.to.name == classObject.name)
				{
					if (!removedEdges.Contains(relationship.edge))
					{
						removedEdges.Add(relationship.edge);
					}

					if (!removedNodes.Contains(relationship.from))
					{
						removedNodes.Add(relationship.from);
					}
				}

				if (relationship.to.name == classObject.name && !removedNodes.Contains(relationship.edge))
				{
					removedNodes.Add(relationship.to);
				}
			}

			if (!removedNodes.Contains(classObject))
			{
				removedNodes.Add(classObject);
			}
		}

		foreach (var removed in removedNodes)
		{
			diagram.RemoveNode(removed);
		}

		foreach (var removed in removedEdges)
		{
			diagram.RemoveEdge(removed);
		}

		diagram.Layout();
	}

	public void Export(GameObject go)
	{
		Dictionary<string, Dictionary<string, List<string>>> data =
			new Dictionary<string, Dictionary<string, List<string>>>();

		foreach (var added in addedClasses)
		{
			FileInfo fileInfo = (FileInfo) added.Value.GetComponent<UNode>().UserData;
			Dictionary<string, List<string>> parameters = new Dictionary<string, List<string>>();
			var methodsText = added.Value.transform.Find("Background").Find("Methods").GetComponent<TextMeshProUGUI>().text;
			var methods = new List<string>(methodsText.Split(new string[] {"()\n"}, StringSplitOptions.None));
			parameters.Add("methods", methods);
			var paths = new List<string>();
			paths.Add(fileInfo.filePath);
			parameters.Add("fsPath", paths);
			data.Add(added.Key, parameters);
		}

		string message = $"{{\"action\":\"generateClasses\", \"args\":{JsonConvert.SerializeObject(data)}}}";
		Debug.Log(message);
		diagram.client.GetComponent<IDEManager>().SendMessage(message);
	}

	IEnumerator SingleClick()
    {
	    clickRoutineRunning = true;
	    yield return new WaitForSecondsRealtime(0.3f);
	    clickRoutineRunning = false;
	    OnRealMouseDown();
    }

    private void OnMouseDown()
    {
	    if (clickRoutineRunning)
	    {
		    StopCoroutine(clickRoutine);
		    clickRoutineRunning = false;
	    }
	    else
	    {
		    clickRoutine = StartCoroutine(SingleClick());
	    }
    }

    void OnRealMouseDown()
    {
	    
    }
}
