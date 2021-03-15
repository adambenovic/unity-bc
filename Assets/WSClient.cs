using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using UnityEngine.Events;
using System.Collections.Concurrent;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.UI;

public class WSClient : MonoBehaviour
{
	[System.Serializable]
	public class MessageEvent : UnityEvent<string, JObject> { }

	public MessageEvent generateDiagramEvent;
	public GameObject classDiagramPrefab;
	public GameObject packageDiagramPrefab;
	public GameObject wordCloudPrefab;
    public GameObject cityPrefab;
    public GameObject diagramGridPrefab;
    public Transform diagrams;
	public Transform misc;
	public Transform grid;

	private WebSocket ws;
	private ConcurrentQueue<Action> queue;

	private HashSet<BackgroundHighlighter> oHighlighted;
	private HashSet<BackgroundHighlighter> bHighlighted;

	private Color _lastColor = Color.black;

	public void Send(string message)
	{
		ws.Send(message);
	}

	// Use this for initialization
	void Start()
	{
		queue = new ConcurrentQueue<Action>();
		oHighlighted = new HashSet<BackgroundHighlighter>();
		bHighlighted = new HashSet<BackgroundHighlighter>();

		ws = new WebSocket("ws://localhost:9000/");
		ws.OnMessage += OnMessage;
		ws.OnOpen += (sender, e) =>
		{
			ws.Send("{\"action\":\"subscribe\", \"args\":{\"events\":[\"generateDiagram\", \"xdeAction\", \"generateWordCloud\", \"generateCity\"]}}");
			Debug.Log("Subscribe");
		};

		generateDiagramEvent.AddListener(OnGenerateDiagram);

		ws.Connect();
		ws.Send("{\"action\":\"echo\"}");
	}

	void OnGenerateDiagram(string type, JObject data)
	{
		Debug.Log("gen");
		if (type == "class")
		{
			CreateClassDiagram(data);
		}
		else if (type == "package")
		{
			CreatePackageDiagram(data);
		}
	}

	void OnGenerateWordCloud(string data)
	{
		var b64_bytes = Convert.FromBase64String(data);
		queue.Enqueue(() =>
		{
			var wcGo = Instantiate(wordCloudPrefab, misc);
			var tex = new Texture2D(1, 1);
			tex.LoadImage(b64_bytes);
			var ri = wcGo.GetComponent<RawImage>();
			ri.texture = tex;
			ri.SetNativeSize();
		});
	}

    void OnGenerateCity(JObject data)
    {
        queue.Enqueue(() =>
        {
            var cdGo = Instantiate(cityPrefab, misc);
            var city = cdGo.GetComponent<City>();
            city.Load(data);
        });
    }

    void OnMessage(object Sender, MessageEventArgs e)
	{
		JObject parsed = JObject.Parse(e.Data);
		string action = parsed.Value<string>("action");
		if (action == "generateDiagram")
		{
			JObject args = parsed.Value<JObject>("args");
			string type = args.Value<string>("type");
			generateDiagramEvent.Invoke(type, args.Value<JObject>("data"));
		}
		else if (action == "xdeAction")
		{
			JObject args = parsed.Value<JObject>("args");
			OnXDEAction(args);
		}
		else if (action == "generateWordCloud")
		{
			JObject args = parsed.Value<JObject>("args");
			OnGenerateWordCloud(args.Value<string>("data"));
		}
        else if(action == "generateCity")
        {
            JObject args = parsed.Value<JObject>("args");
            OnGenerateCity(args.Value<JObject>("data"));
        }
	}

	void OnXDEAction(JObject args)
	{
		string type = args.Value<string>("type");
		if (type == "highlight")
		{
			HighlightSelected(args);
		}
		else if (type == "multipleHighlight")
		{
			CtrlHighlightSelected(args);
		}
		else if (type == "showSearchResults")
		{
			ShowSearchResults(args.Value<string>("results"));
		}
	}

	void HighlightSelected(JObject args)
	{
		queue.Enqueue(() =>
		{
			string path = args.Value<string>("path");
			int line = args.Value<int>("line");
			UnhighlightOutlines();
			Highlight(path, line, true, false);
		});
	}
	
	void CtrlHighlightSelected(JObject args)
	{
		queue.Enqueue(() =>
		{
			string path = args.Value<string>("path");
			int line = args.Value<int>("line");
			Highlight(path, line, true, false);
		});
	}

	void CreateClassDiagram(JObject data)
	{
		queue.Enqueue(() =>
		{
			var cdGo = Instantiate(classDiagramPrefab, diagrams);
			var cdGraph = cdGo.GetComponent<ClassDiagram>();
			cdGraph.client = this;
			cdGraph.Load(data);

			var dGrid = Instantiate(diagramGridPrefab, grid);
			var cdGrid = dGrid.GetComponent<GridManager>();
			cdGrid.GridGenerate(cdGraph);
		});
	}

	void CreatePackageDiagram(JObject data)
	{
		queue.Enqueue(() =>
		{
			var pdGo = Instantiate(packageDiagramPrefab, diagrams);
			var pdGraph = pdGo.GetComponent<PackageDiagram>();
			pdGraph.client = this;
			pdGraph.Load(data);
		});
	}

	void UnhighlightOutlines()
	{
		foreach (var highlighter in oHighlighted)
		{
			highlighter?.UnhighlightOutline();
		}
		_lastColor = Color.black;
		oHighlighted.Clear();
	}

	void UnhighlightBackgrounds()
	{
		foreach (var highlighter in bHighlighted)
		{
			highlighter?.UnhighlightBackground();
		}
		bHighlighted.Clear();
	}

	public void Highlight(string path, int line, bool outline, bool background, bool isNextDataset = false)
	{
		foreach (var cd in diagrams.GetComponentsInChildren<Diagram>())
		{
			JArray objs = cd.FileMap.Value<JArray>(path.ToLower());
			if (objs != null)
			{
				foreach (JObject obj in objs)
				{
					Debug.Log("fromLineNo: " + obj.Value<int>("fromLineNo"));
					var fromLine = obj.Value<int>("fromLineNo");
					var toLine = obj.Value<int>("toLineNo");
					if (line >= fromLine && line <= toLine)
					{
						var qname = obj.Value<string>("qname");
						var cls = cd.Find(qname);
						if (cls != null)
						{
							var highlighter = cls.GetComponent<BackgroundHighlighter>();
							if (outline)
							{
								if (highlighter.IsHighlightedOutline())
								{
									highlighter.UnhighlightOutline();
									oHighlighted.Remove(highlighter);
								}
								else
								{
									if (isNextDataset)
									{
										_lastColor = UnityEngine.Random.ColorHSV();
									}
									highlighter.HighlightOutline(_lastColor);
									oHighlighted.Add(highlighter);
								}
							}
							if (background)
							{
								highlighter.HighlightBackground();
								bHighlighted.Add(highlighter);
							}
						}
					}
				}
			}
		}
	}

	void ShowSearchResults(string searchResults)
	{
		queue.Enqueue(() =>
		{
			using (StringReader sr = new StringReader(searchResults))
			{

				string path = null;
				while (sr.Peek() >= 0)
				{
					Regex pathRegex = new Regex(@"^(\S+)");
					Regex lineRegex = new Regex(@"^\s+\d+,(\d+):");
					string line = sr.ReadLine();

					var match = pathRegex.Match(line);
					if (match.Success)
					{
						path = match.Groups[1].Value;
					}
					else if (path != null)
					{
						match = lineRegex.Match(line);
						if (match.Success)
						{
							//highlight object based on path and line
							int lineNumber = 0;
							if (Int32.TryParse(match.Groups[1].Value, out lineNumber))
							{
								Highlight(path, lineNumber, false, true);
							}
						}
					}
				}
			}
		});
	}

	// Update is called once per frame
	void Update()
	{
		Action queuedAction;
		queue.TryDequeue(out queuedAction);
		queuedAction?.Invoke();
	}

	private void OnDestroy()
	{
		ws.Close();
	}
}
