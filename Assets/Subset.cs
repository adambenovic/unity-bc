using System.Collections.Generic;
using UnityEngine;

public class Subset
{
    private List<GameObject> _gameObjects;
    private readonly Color _color;

    public Subset()
    {
        _gameObjects = new List<GameObject>();
        _color = Random.ColorHSV();
    }

    public void AddToSubset(GameObject go)
    {
        _gameObjects.Add(go);
    }

    public void RemoveFromSubset(GameObject go)
    {
        if (IsInSubset(go)) _gameObjects.Remove(go);
    }

    public bool IsInSubset(GameObject go)
    {
        if (_gameObjects.Contains(go)) return true;

        return false;
    }

    public Color GetColor()
    {
        return _color;
    }

    public void Clear()
    {
        _gameObjects.Clear();
    }
    public List<GameObject> GetObjects()
    {
        return _gameObjects;
    }
}