using Newtonsoft.Json;
using UnityEngine;

public class IDEManager : MonoBehaviour
{
	private Subset selected;

	public void Start()
	{
		selected = new Subset();
	}

	public void ShowInEditor(GameObject go)
	{
		FileInfo fileInfo = (FileInfo)go.GetComponent<UNode>().UserData;
		GetComponent<WSClient>().Send($"{{\"action\":\"ideAction\", \"args\":{{\"type\":\"goto\", \"path\":{JsonConvert.ToString(fileInfo.filePath)}, \"fromLine\":{fileInfo.lineFrom}, \"toLine\":{fileInfo.lineTo}}}}}");
	}

	public void SendMessage(string message)
	{
		GetComponent<WSClient>().Send(message);
	}

	public Subset GetSelected()
	{
		return selected;
	}

	public void Select(GameObject go)
	{
		var highlighter = go.GetComponent<BackgroundHighlighter>();
		var wasHighlighted = highlighter.IsHighlightedOutline() && highlighter.highlightColor != highlighter.myGreen;
		var numObjects = selected.GetObjects().Count;

		foreach (var classToRemove in selected.GetObjects())
		{
			classToRemove.GetComponent<BackgroundHighlighter>().UnhighlightOutline();
		}

		selected.Clear();
		selected.AddToSubset(go);
		if (!wasHighlighted || numObjects > 1)
			highlighter.HighlightOutline(Color.black);
	}

	public void SelectMultiple(GameObject go, bool isNextDataset)
	{
		if (CheckHighlighted(go))
			return;
		selected.AddToSubset(go);
		go.GetComponent<BackgroundHighlighter>().HighlightOutline(Color.black);
	}

	private bool CheckHighlighted(GameObject go)
	{
		BackgroundHighlighter highlighter = go.GetComponent<BackgroundHighlighter>();
		if (highlighter.IsHighlightedOutline() && selected.IsInSubset(go))
		{
			highlighter.UnhighlightOutline();
			selected.RemoveFromSubset(go);

			return true;
		}

		return false;
	}
}
