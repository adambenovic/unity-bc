///////////////////////////////////////////////////////////
//  MessageKind.cs
//  Implementation of the Class MessageKind
//  Generated by Enterprise Architect
//  Created on:      04-Oct-2018 16:51:33
//  Original author: Iva
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace UML.Interactions {
	/// <summary>
	/// This is an enumerated type that identifies the type of Message.
	/// </summary>
	public enum MessageKind : int {

		/// <summary>
		/// sendEvent and receiveEvent are present
		/// </summary>
		complete,
		/// <summary>
		/// sendEvent present and receiveEvent absent
		/// </summary>
		lost,
		/// <summary>
		/// sendEvent absent and receiveEvent present
		/// </summary>
		found,
		/// <summary>
		/// sendEvent and receiveEvent absent (should not appear)
		/// </summary>
		unknown

	}//end MessageKind

}//end namespace Interactions