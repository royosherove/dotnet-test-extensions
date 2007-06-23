/// Created by Roy Osherove, http://www.iserializable.com
/// ------------------------------------------------------
using System;
using System.Diagnostics;
using XtUnit.Framework.Internal;

namespace XtUnit.Framework
{

	/// <summary>
	///     Inherit from this attribute to create your own custom attributes
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property,AllowMultiple=true,Inherited=true)]
	public abstract class TestProcessingAttributeBase : ProcessingAttributeBase
	{
		protected void OutputDebugMessage (string message)
		{
			Debug.WriteLine(message);
			//Trace.WriteLine(message);
			//Console.WriteLine(message);
		}

	}
	

}
