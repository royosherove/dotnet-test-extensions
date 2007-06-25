using System;

namespace XtUnit.Framework.Internal
{
	/// <summary>
	///     Base class for objects that can carry Custom attributes 
	///     which can do pre and post processing.
	///     You MUST inherit your class from this object if you want to use 
	///     your custom attributes.
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	[Intercept]
	public class InterceptableObject:ContextBoundObject
	{
	}
}