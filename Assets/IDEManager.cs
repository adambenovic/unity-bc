using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class IDEManager : MonoBehaviour
{
	public void ShowInEditor(GameObject go)
	{
		FileInfo fileInfo = (FileInfo)go.GetComponent<UNode>().UserData;
		GetComponent<WSClient>().Send($"{{\"action\":\"ideAction\", \"args\":{{\"type\":\"goto\", \"path\":{JsonConvert.ToString(fileInfo.filePath)}, \"fromLine\":{fileInfo.lineFrom}, \"toLine\":{fileInfo.lineTo}}}}}");
	}
	
	private List<Subset> _datasets = new List<Subset>();
	public void Manage(GameObject go, bool isNextDataset)
	{
		if(CheckHighlighted(go))
			return;
		var index = AddToSet(go, isNextDataset);
		go.GetComponent<BackgroundHighlighter>().HighlightOutline(_datasets[index].GetColor());
	}

	/**
	 * Return index of Subset to which GameObject was added.
	 */
	private int AddToSet(GameObject go, bool isNextDataset)
	{
		if (isNextDataset || !_datasets.Any())
		{
			Subset newSubset = new Subset();
			newSubset.AddToSubset(go);
			_datasets.Add(newSubset);
		}
		else
		{
			_datasets[_datasets.Count - 1].AddToSubset(go);
		}

		return _datasets.Count - 1;
	}

	/**
	 * Return index of Subset in which GameObject is contained.
	 */
	private int FindSubset(GameObject go)
	{
		int index;
		for (index = 0; index < _datasets.Count; index++)
		{
			if (_datasets[index].IsInSubset(go))
				break;
		}

		return index;
	}

	private bool CheckHighlighted(GameObject go)
	{
		BackgroundHighlighter bg = go.GetComponent<BackgroundHighlighter>();
		if (bg.IsHighlightedOutline())
		{
			bg.UnhighlightOutline();
			int index = FindSubset(go);
			_datasets[index].RemoveFromSubset(go);

			return true;
		}

		return false;
	}
}
