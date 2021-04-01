using System.Collections.Generic;
using UnityEngine;

public class RawClass
{
	public string name;
	public List<string> methods;
	public List<string> associations;

	public RawClass(string name, List<string> methods, List<string> associations)
	{
		this.name = name;
		this.methods = methods;
		this.associations = associations;
	}
}
