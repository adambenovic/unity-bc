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
    public GameObject prefab;
    private Transform gridUnits;
    
    public GameObject plusButton;
    public GameObject minusButton;
    public GameObject openFileButton;
    public GameObject exportButton;
    public GameObjectEvent triggerSelectAction;
    public GameObjectEvent triggerPlusAction;
    public GameObjectEvent triggerMinusAction;
    public GameObjectEvent triggerOpenFileButton;
    public GameObjectEvent triggerExportButton;
    private Coroutine clickRoutine;
    private bool clickRoutineRunning;

    private ClassDiagram diagram;
    private Dictionary<string, RawClass> rawClassesFromFile = new Dictionary<string, RawClass>();
    private Dictionary<string, GameObject> classesFromFile = new Dictionary<string, GameObject>();
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
        var xStart = graphPosition.xMin;
        var yStart = graphPosition.yMax + 25;

        GameObject plus = Instantiate(plusButton, gridUnits);
	    plus.GetComponent<GridManager>().triggerPlusAction.AddListener(plusAction);
	    plus.transform.position = new Vector3(xStart, yStart);
	    
	    GameObject minus = Instantiate(minusButton, gridUnits);
	    minus.GetComponent<GridManager>().triggerMinusAction.AddListener(minusAction);
	    minus.transform.position = new Vector3(xStart + 15, yStart);
	    
	    GameObject open = Instantiate(openFileButton, gridUnits);
	    open.GetComponent<GridManager>().triggerOpenFileButton.AddListener(OpenFile);
	    open.transform.position = new Vector3(xStart + 45, yStart);

	    GameObject export = Instantiate(exportButton, gridUnits);
	    export.GetComponent<GridManager>().triggerExportButton.AddListener(Export);
	    export.transform.position = new Vector3(xStart + 85, yStart);
    }

    public void OpenFile(GameObject go)
    {
	    var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "./Examples/", JsonExtension, false);

	    if (paths.Length > 0)
	    {
		    rawClassesFromFile.Clear();
		    classesFromFile.Clear();
		    rawClassesFromFile = SerializationManager.LoadElementsFromFile(paths[0]);
		    
		    if (rawClassesFromFile == null || rawClassesFromFile.Count < 1)
		    {
			    return;
		    }

		    Regex reg = new Regex(@".*\..*\.(.*)");

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

    private void plusAction(GameObject go)
    {
	    List<string> toBeRemoved = new List<string>();
	    foreach (var classObject in rawClassesFromFile)
	    {
		    var node = diagram.AddNode();
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
				    } else if (relationship.type == "specialization")
				    {
					    prefab = diagram.specialization;
				    } else if(relationship.type == "implements")
				    {
					    prefab = diagram.implements;
				    }

				    relationship.edge = diagram.AddEdge(relationship.from, relationship.to, prefab);
			    } else if (fi.filePath.IsNullOrEmpty() && (relationship.from.name == classObject.Value.name || relationship.to.name == classObject.Value.name))
			    {
				    string fromPath = getFilePathFromNode(diagram, relationship.from.name, path);
				    string toPath = getFilePathFromNode(diagram, relationship.to.name, path);
				    if (fromPath != path)
				    {
					    path = fromPath;
				    } else if (toPath != path)
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
		    FileInfo fi = (FileInfo)diagram.Nodes[key].GetComponent<UNode>().UserData;
		    if (!fi.filePath.IsNullOrEmpty())
		    {
			    path = fi.filePath;
		    }
	    }
	    catch (Exception e) {}

	    return path;
    }

    private void minusAction(GameObject go)
    {
	    List<GameObject> removedNodes = new List<GameObject>();
	    List<GameObject> removedEdges = new List<GameObject>();

	    foreach (var classObject in classesFromFile)
	    {
		    foreach (var relationship in diagram.Relationshipis)
		    {
			    if (relationship.from.name == classObject.Value.name || relationship.to.name == classObject.Value.name)
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

			    if (relationship.to.name == classObject.Value.name && !removedNodes.Contains(relationship.edge))
			    {
				    removedNodes.Add(relationship.to);
			    }
		    }

		    if (!removedNodes.Contains(classObject.Value))
		    {
			    removedNodes.Add(classObject.Value);
		    }
	    }

	    foreach (var removed in removedNodes)
	    {
		    diagram.RemoveNode(removed);
		    removed.SetActive(false);
	    }

	    foreach (var removed in removedEdges)
	    {
		    diagram.RemoveEdge(removed);
		    removed.SetActive(false);
	    }

	    diagram.Layout();
    }

    public void Export(GameObject go)
    {
	    Dictionary<string, Dictionary<string, List<string>>> data = new Dictionary<string, Dictionary<string, List<string>>>();

	    foreach (var added in addedClasses)
	    {
		    FileInfo fileInfo = (FileInfo)added.Value.GetComponent<UNode>().UserData;
		    Dictionary<string, List<string>> parameters = new Dictionary<string, List<string>>();
		    var methodsText = added.Value.transform.Find("Background").Find("Methods").GetComponent<TextMeshProUGUI>().text;
		    var methods = new List<string>(methodsText.Split(new string[] { "()\n" }, StringSplitOptions.None));
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
	    triggerPlusAction.Invoke(gameObject);
	    triggerMinusAction.Invoke(gameObject);
	    triggerSelectAction.Invoke(gameObject);
	    triggerOpenFileButton.Invoke(gameObject);
	    triggerExportButton.Invoke(gameObject);
    }
}
