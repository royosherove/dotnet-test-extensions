using NUnit.Framework;
using XtUnit.Framework.Internal;

namespace XtUnit.Framework
{
	/// <summary>
	///     This is the base class for all the test fixtures you will
	///     have in your project. You MUST inherit from this in order 
	///     for the custom attributes to work. No other special action is needed.
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	[TestFixture]
	public class TestFixtureBase:InterceptableObject
	{

	}
}
