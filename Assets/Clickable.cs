using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class GameObjectEvent : UnityEvent<GameObject> { };
[Serializable] public class MultiGameObjectEvent : UnityEvent<GameObject, bool> { };

public class Clickable : MonoBehaviour
{
	public GameObjectEvent triggerAction;
	public float doubleClickLimit = 0.3f;

	private Vector3 screenPoint;
	private Vector3 offset;

	private Coroutine clickRoutine;
	private bool clickRoutineRunning = false;
	private bool doubleClick = false;

	public MultiGameObjectEvent triggerCtrlAction;
	private static bool _ctrlDown;
	private static float _nextDatasetLimit = 3.0f;
	private static float _nextDataset;
	private static bool _isNextDataset;
	private static bool _isDone;

	IEnumerator SingleClick()
	{
		clickRoutineRunning = true;
		yield return new WaitForSecondsRealtime(doubleClickLimit);
		clickRoutineRunning = false;
		OnRealMouseDown();
	}

	private void OnMouseDown()
	{
		if (clickRoutineRunning)
		{
			StopCoroutine(clickRoutine);
			clickRoutineRunning = false;
			OnDoubleClick();
		}
		else
		{
			clickRoutine = StartCoroutine(SingleClick());
		}
	}

	private void OnDoubleClick()
	{
		doubleClick = true;
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}

	void OnRealMouseDown()
	{
		if (_ctrlDown)
		{
			Debug.Log("isNextDataset = " + _isNextDataset);
			triggerCtrlAction.Invoke(gameObject, _isNextDataset);
			_isDone = true;
			Debug.Log("Is Done!");
		}
			
		else
			triggerAction.Invoke(gameObject);
	}

	void OnMouseUp()
	{
		doubleClick = false;
	}

	void OnMouseDrag()
	{
		if (doubleClick == false)
		{
			return;
		}
		Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
		transform.position = cursorPosition;
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			Debug.Log("We setting true");
			_ctrlDown = true;
			_isNextDataset = true;
			_nextDataset = Time.time + _nextDatasetLimit;
		}
		
		if (Input.GetKeyUp(KeyCode.LeftControl))
		{
			_ctrlDown = false;
		}
		
		if (Input.GetMouseButtonDown(0))
		{
			if (_ctrlDown)
			{
				_nextDataset = Time.time + _nextDatasetLimit;
			}
		}
		
		if (_isNextDataset && _isDone)
		{
			Debug.Log("Everything falls false");
			_isNextDataset = false;
			_isDone = false;
		}
		
		if (_ctrlDown && Time.time > _nextDataset && !_isNextDataset)
		{
			Debug.Log("We update to true again");
			_isNextDataset = true;
			_nextDataset = Time.time + _nextDatasetLimit;
		}
	}
}