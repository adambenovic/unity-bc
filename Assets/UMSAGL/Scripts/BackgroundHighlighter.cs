﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundHighlighter : MonoBehaviour {

	public Color highlightColor;
	public Color defaultColor = Color.black;
	public Color myGreen = new Color(0, 102, 0);

	private void Awake()
	{
		defaultColor = GetComponentInChildren<Image>().color;
	}

	public void HighlightOutline(Color color)
	{
		Outline outline = GetComponentInChildren<Outline>();
		outline.effectColor = color;
		outline.enabled = true;
		highlightColor = color;
	}

	public void HighlightBackground()
	{
		GetComponentInChildren<Image>().color = highlightColor;
	}

	public void UnhighlightOutline()
	{
		Outline outline = GetComponentInChildren<Outline>();
		outline.effectColor = defaultColor;
		outline.enabled = false;
		highlightColor = defaultColor;
	}

	public void UnhighlightBackground()
	{
		GetComponentInChildren<Image>().color = defaultColor;
	}

	public bool IsHighlightedOutline()
	{
		return GetComponentInChildren<Outline>().enabled;
	}

}
