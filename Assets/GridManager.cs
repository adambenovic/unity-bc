using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GameObject prefab;
    private Transform gridUnits;
    
    public GameObject plusButton;
    public GameObject minusButton;
    public GameObject clearButton;
    public GameObjectEvent triggerSelectAction;
    public GameObjectEvent triggerPlusAction;
    public GameObjectEvent triggerMinusAction;
    public GameObjectEvent triggerClearAction;

    public List<List<Tuple<GameObject, GameObject>>> sets;
    private List<GameObject> resultSet = new List<GameObject>();
    private List<int> selected;

    public void GridGenerate()
    {
	    selected = new List<int>();
	    gridUnits =  transform.Find("GridUnits");
	    sets = new List<List<Tuple<GameObject, GameObject>>>();
        var graphPosition = GameObject.FindGameObjectWithTag("ClassDiagram").GetComponent<RectTransform>().rect;
        var xStart = graphPosition.width;
        var yStart = graphPosition.height;
        var classes = GameObject.FindGameObjectsWithTag("Class");
        Vector3 lastPos;
        Vector3 lastSize;
        
        Debug.Log("no classes = " + classes.Length);
        
        var lifelines = SerializationManager.LoadElementsFromFile("./Adam.json");
        
        Regex reg = new Regex(@".*\..*\.(.*)");
        
        foreach (var classObject in classes)
        {
	        var match = reg.Match(classObject.name);
	        if (!lifelines.Contains(match.Groups[1].Value))
	        {
		        classObject.SetActive(false);
	        }
        }
        
        sets.Add(new List<Tuple<GameObject, GameObject>>());
        sets.Add(new List<Tuple<GameObject, GameObject>>());
        sets.Add(new List<Tuple<GameObject, GameObject>>());
        sets.Add(new List<Tuple<GameObject, GameObject>>());
        
	    sets[0].Add(new Tuple<GameObject, GameObject>(classes[0], Instantiate(prefab, gridUnits)));
	    sets[0].Add(new Tuple<GameObject, GameObject>(classes[2], Instantiate(prefab, gridUnits)));
	    
	    sets[1].Add(new Tuple<GameObject, GameObject>(classes[1], Instantiate(prefab, gridUnits)));
	    sets[1].Add(new Tuple<GameObject, GameObject>(classes[3], Instantiate(prefab, gridUnits)));
	    
	    sets[2].Add(new Tuple<GameObject, GameObject>(classes[0], Instantiate(prefab, gridUnits)));
	    sets[2].Add(new Tuple<GameObject, GameObject>(classes[1], Instantiate(prefab, gridUnits)));
	    sets[2].Add(new Tuple<GameObject, GameObject>(classes[2], Instantiate(prefab, gridUnits)));
	    sets[2].Add(new Tuple<GameObject, GameObject>(classes[3], Instantiate(prefab, gridUnits)));
	    
	    sets[3].Add(new Tuple<GameObject, GameObject>(classes[0], Instantiate(prefab, gridUnits)));
	    sets[3].Add(new Tuple<GameObject, GameObject>(classes[1], Instantiate(prefab, gridUnits)));
	    sets[3].Add(new Tuple<GameObject, GameObject>(classes[2], Instantiate(prefab, gridUnits)));
	    sets[3].Add(new Tuple<GameObject, GameObject>(classes[3], Instantiate(prefab, gridUnits)));
	    sets[3].Add(new Tuple<GameObject, GameObject>(classes[4], Instantiate(prefab, gridUnits)));
	    sets[3].Add(new Tuple<GameObject, GameObject>(classes[5], Instantiate(prefab, gridUnits)));
	    sets[3].Add(new Tuple<GameObject, GameObject>(classes[6], Instantiate(prefab, gridUnits)));

	    for (int i = 0; i < sets.Count; i++)
        {
	        for (int j = 0; j < sets[i].Count; j++)
	        {
		        var go = sets[i][j].Item2;
		        go.GetComponent<GridManager>().triggerSelectAction.AddListener(selectAction);
		        go.name = sets[i][j].Item1.name;
		        var background = go.transform.Find("Background");
		        var header = background.Find("Header");
		        header.GetComponent<TextMeshProUGUI>().text = go.name;
		        lastSize = header.GetComponent<BoxCollider>().bounds.size;
		        lastPos = new Vector3(xStart + lastSize.x * i, yStart + -lastSize.y * j);
		        go.transform.position = lastPos;
	        }
        }
	    
	    GameObject plus = Instantiate(plusButton, gridUnits);
	    plus.GetComponent<GridManager>().triggerPlusAction.AddListener(plusAction);
	    plus.transform.position = new Vector3(xStart + 50, yStart + 15);
	    
	    GameObject minus = Instantiate(minusButton, gridUnits);
	    minus.GetComponent<GridManager>().triggerMinusAction.AddListener(minusAction);
	    minus.transform.position = new Vector3(xStart + 65, yStart + 15);
	    
	    GameObject clear = Instantiate(clearButton, gridUnits);
	    clear.GetComponent<GridManager>().triggerClearAction.AddListener(clearAction);
	    clear.transform.position = new Vector3(xStart + 85, yStart + 15);
    }

    private void plusAction(GameObject go)
    {
	    Debug.Log("Plus action");
	    for (int i = 0; i < selected.Count; i++)
	    {
		    for (int j = 0; j < sets[selected[i]].Count; j++)
		    {
			    if (!resultSet.Contains(sets[selected[i]][j].Item1))
			    {
				    resultSet.Add(sets[selected[i]][j].Item1);
				    sets[selected[i]][j].Item2.GetComponent<BackgroundHighlighter>().HighlightOutline(new Color(0, 153, 0));
				    sets[selected[i]][j].Item1.GetComponent<BackgroundHighlighter>().HighlightOutline(new Color(0, 102, 255));
			    }
			    else
			    {
				    sets[selected[i]][j].Item2.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
			    }
		    }
	    }
	    
	    selected.Clear();
    }
    
    private void minusAction(GameObject go)
    {
	    Debug.Log("Minus action");
	    for (int i = 0; i < selected.Count; i++)
	    {
		    for (int j = 0; j < sets[selected[i]].Count; j++)
		    {
			    if (resultSet.Contains(sets[selected[i]][j].Item1))
			    {
				    resultSet.Remove(sets[selected[i]][j].Item1);
				    sets[selected[i]][j].Item2.GetComponent<BackgroundHighlighter>().HighlightOutline(new Color(153, 0, 0));
				    sets[selected[i]][j].Item1.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
			    }
			    else
			    {
				    sets[selected[i]][j].Item2.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
			    }
		    }
	    }
	    
	    selected.Clear();
    }
    
    private void clearAction(GameObject go)
    {
	    Debug.Log("Clear action");
	    foreach (var list in sets)
	    {
		    foreach (var tuple in list)
		    {
			    tuple.Item1.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
			    tuple.Item2.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
		    }
	    }
	    
	    for (int i = 0; i < selected.Count; i++)
	    {
		    for (int j = 0; j < sets[selected[i]].Count; j++)
		    {
			    sets[selected[i]][j].Item1.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
			    sets[selected[i]][j].Item2.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
		    }
	    }

	    foreach (var obj in resultSet)
	    {
		    obj.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
	    }
	    
	    selected.Clear();
	    resultSet.Clear();
    }

    public void selectAction(GameObject go)
    {
	    Debug.Log("Select action");
	    var index = searchInSets(go);
	    if (index != null)
	    {
		    var setIndex = (int) index;
		    if (selected.Contains(setIndex))
		    {
			    selected.Remove(setIndex);
			    for (int i = 0; i < sets[setIndex].Count; i++)
			    {
				    sets[setIndex][i].Item2.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
			    }
			    
			    return;
		    }
		    selected.Add(setIndex);
		    for (int i = 0; i < sets[setIndex].Count; i++)
		    {
			    sets[setIndex][i].Item2.GetComponent<BackgroundHighlighter>().HighlightOutline(Color.black);
		    }
	    }
    }
    
    private int? searchInSets(GameObject go)
    {
	    for (int i = 0; i < sets.Count; i++)
	    {
		    for (int j = 0; j < sets[i].Count; j++)
		    {
			    if (sets[i][j].Item1 == go || sets[i][j].Item2 == go)
				    return i;
		    }
	    }

	    return null;
    }
    
    private Coroutine clickRoutine;
    private bool clickRoutineRunning;
    
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
	    triggerClearAction.Invoke(gameObject);
    }
}
