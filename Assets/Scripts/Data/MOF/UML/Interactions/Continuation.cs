///////////////////////////////////////////////////////////
//  Continuation.cs
//  Implementation of the Class Continuation
//  Generated by Enterprise Architect
//  Created on:      04-Oct-2018 16:51:28
//  Original author: Iva
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



using UML.Interactions;
namespace UML.Interactions {
	/// <summary>
	/// A Continuation is a syntactic way to define continuations of different branches
	/// of an alternative CombinedFragment. Continuations are intuitively similar to
	/// labels representing intermediate points in a flow of control.
	/// </summary>
	public class Continuation : InteractionFragment {

		/// <summary>
		/// True: when the Continuation is at the end of the enclosing InteractionFragment
		/// and False when it is in the beginning.
		/// </summary>
		public bool setting = true;

		public Continuation(){

		}

		~Continuation(){

		}

	}//end Continuation

}//end namespace Interactions