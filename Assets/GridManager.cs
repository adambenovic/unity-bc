using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using SFB;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject prefab;
    private Transform gridUnits;
    
    public GameObject plusButton;
    public GameObject minusButton;
    public GameObject openFileButton;
    public GameObjectEvent triggerSelectAction;
    public GameObjectEvent triggerPlusAction;
    public GameObjectEvent triggerMinusAction;
    public GameObjectEvent triggerOpenFileButton;    
    private Coroutine clickRoutine;
    private bool clickRoutineRunning;

    private ClassDiagram diagram;
    private Dictionary<string, List<string>> rawClassesFromFile = new Dictionary<string, List<string>>();
    private Dictionary<string, GameObject> classesFromFile = new Dictionary<string, GameObject>();

    private static readonly ExtensionFilter[] JsonExtension = new ExtensionFilter[]
    {
	    new ExtensionFilter("JSON", "json"),
    };

    public void GridGenerate(ClassDiagram diagram)
    {
	    this.diagram = diagram;
	    gridUnits = transform.Find("GridUnits");
	    var graphPosition = diagram.GetComponent<RectTransform>().rect;
        var xStart = graphPosition.width;
        var yStart = graphPosition.height;

        GameObject plus = Instantiate(plusButton, gridUnits);
	    plus.GetComponent<GridManager>().triggerPlusAction.AddListener(plusAction);
	    plus.transform.position = new Vector3(xStart + 50, yStart + 15);
	    
	    GameObject minus = Instantiate(minusButton, gridUnits);
	    minus.GetComponent<GridManager>().triggerMinusAction.AddListener(minusAction);
	    minus.transform.position = new Vector3(xStart + 65, yStart + 15);
	    
	    GameObject open = Instantiate(openFileButton, gridUnits);
	    open.GetComponent<GridManager>().triggerOpenFileButton.AddListener(OpenFile);
	    open.transform.position = new Vector3(xStart + 95, yStart + 15);
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
			    // TODO add error message popup
			    return;
		    }

		    Regex reg = new Regex(@".*\..*\.(.*)");

		    foreach (var classObject in diagram.Nodes)
		    {
			    var match = reg.Match(classObject.Key);
			    if (rawClassesFromFile.ContainsKey(match.Groups[1].Value))
			    {
				    classesFromFile.Add(classObject.Key, classObject.Value);
				    rawClassesFromFile.Remove(match.Groups[1].Value);
			    }
			    else if (rawClassesFromFile.ContainsKey(classObject.Key))
			    {
				    classesFromFile.Add(classObject.Key, classObject.Value);
				    rawClassesFromFile.Remove(classObject.Key);
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

		    foreach (var method in classObject.Value)
		    {
			    methods.GetComponent<TextMeshProUGUI>().text += $"{method}()\n";
		    }

		    diagram.AddNode(node.name, node);
		    classesFromFile.Add(node.name, node);
		    toBeRemoved.Add(node.name);
	    }

	    foreach (var toRemove in toBeRemoved)
	    {
		    rawClassesFromFile.Remove(toRemove);
	    }

	    foreach (var classObject in classesFromFile)
	    {
			diagram.AddNode(classObject.Key, classObject.Value);

			foreach (var relationship in diagram.Relationshipis)
		    {
			    if (relationship.type == "specialization")
			    {
				    if (relationship.from.name == classObject.Value.name && !relationship.edge.activeSelf)
				    {
					    Debug.Log("relationship from " + relationship.from.name + " to " + relationship.to.name);
					    relationship.edge = diagram.AddEdge(relationship.from, relationship.to, diagram.specialization);
				    }
			    }
		    }
	    }

	    diagram.UpdateGraph();
	    diagram.Layout();
    }

    private void minusAction(GameObject go)
    {
	    List<GameObject> removedNodes = new List<GameObject>();
	    List<GameObject> removedEdges = new List<GameObject>();

	    foreach (var classObject in classesFromFile)
	    {
		    foreach (var relationship in diagram.Relationshipis)
		    {
			    if (relationship.type == "specialization")
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
    }
}
