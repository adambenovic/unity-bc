using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using SFB;

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

    private List<GameObject> classesFromFile = new List<GameObject>();
    private List<int> selected;
    
    private List<string> classNamesFromFile;

    private static readonly ExtensionFilter[] JsonExtension = new ExtensionFilter[]
    {
	    new ExtensionFilter("JSON", "json"),
    };
    
    private Coroutine clickRoutine;
    private bool clickRoutineRunning;

    public void GridGenerate()
    {
	    gridUnits =  transform.Find("GridUnits");
	    var graphPosition = GameObject.FindGameObjectWithTag("ClassDiagram").GetComponent<RectTransform>().rect;
        var xStart = graphPosition.width;
        var yStart = graphPosition.height;
        Vector3 lastPos;
        Vector3 lastSize;

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
		    classNamesFromFile = SerializationManager.LoadElementsFromFile(paths[0]);
		    
		    if (classNamesFromFile == null || classNamesFromFile.Count < 1)
		    {
			    // TODO add error message popup
			    return;
		    }
		    
		    var classes = GameObject.FindGameObjectsWithTag("Class");
	    
		    Regex reg = new Regex(@".*\..*\.(.*)");
        
		    foreach (var classObject in classes)
		    {
			    var match = reg.Match(classObject.name);
			    if (classNamesFromFile.Contains(match.Groups[1].Value))
			    {
				    classesFromFile.Add(classObject);
				    classNamesFromFile.Remove(match.Groups[1].Value);
			    }
		    }

		    foreach (var classNamme in classNamesFromFile)
		    {
			    Debug.Log("Tried to work with not yet existing class " + classNamme);
		    }
	    }
    }

    private void plusAction(GameObject go)
    {
	    Debug.Log("Plus action");
    }
    
    private void minusAction(GameObject go)
    {
	    Debug.Log("Minus action");
	    foreach (var classObject in classesFromFile)
	    {
		    classObject.SetActive(false);
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
