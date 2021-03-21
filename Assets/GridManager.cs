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

    private ClassDiagram diagram;

    private Dictionary<string, GameObject> classesFromFile = new Dictionary<string, GameObject>();
    private List<int> selected;
    
    private Dictionary<string, List<string>> rawClassesFromFile;

    private static readonly ExtensionFilter[] JsonExtension = new ExtensionFilter[]
    {
	    new ExtensionFilter("JSON", "json"),
    };
    
    private Coroutine clickRoutine;
    private bool clickRoutineRunning;

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
		    }

		    foreach (var classFromFile in rawClassesFromFile)
		    {
			    Debug.Log("Tried to work with not yet existing class " + classFromFile.Key);
		    }
	    }
    }

    private void plusAction(GameObject go)
    {
	    Debug.Log("Plus action");
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

		    diagram.AddNode(classObject.Key, node);
		    classesFromFile.Add(classObject.Key, node);
		    toBeRemoved.Add(classObject.Key);
	    }

	    foreach (var toRemove in toBeRemoved)
	    {
		    rawClassesFromFile.Remove(toRemove);
	    }

	    foreach (var classObject in classesFromFile)
	    {
		    foreach (var relationship in diagram.Relationshipis)
		    {
			    if (relationship.type == "specialization")
			    {
				    if (relationship.from.name == classObject.Value.name)
				    {
					    relationship.edge.SetActive(true);
				    }
			    }
		    }
		    
		    classObject.Value.SetActive(true);
	    }

	    diagram.Layout();
    }

    private void minusAction(GameObject go)
    {
	    Debug.Log("Minus action");
	    foreach (var classObject in classesFromFile)
	    {
		    foreach (var relationship in diagram.Relationshipis)
		    {
			    if (relationship.type == "specialization")
			    {
				    if (relationship.from.name == classObject.Value.name || relationship.to.name == classObject.Value.name)
				    {
					    relationship.edge.SetActive(false);
					    relationship.from.SetActive(false);
					    Debug.Log("from: " + relationship.from.name);
				    }

				    if (relationship.to.name == classObject.Value.name)
				    {
					    relationship.to.SetActive(false);
					    Debug.Log("to: " + relationship.to.name);
				    }
			    }
		    }

		    classObject.Value.SetActive(false);
	    }
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
