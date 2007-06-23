using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace XtUnit.Framework.Internal
{

	/// <summary>
	///     This is the base class for custom attributes
	///     that you would like to add to your tests.
	///     just inherit from it and implement the two simple abstract methods.
	///     then you can start using your new attribute right away in your tests.
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property,AllowMultiple=true,Inherited=true)]
	public abstract class ProcessingAttributeBase : Attribute
	{
		protected IMethodCallMessage methodCallMessage=null;
		protected MarshalByRefObject methodCallTarget=null;
		protected IMethodReturnMessage methodReturnMessage=null;
	    private Type declaringType;
	    private MethodBase declaringMethod;

	    public Type DeclaringType
	    {
	        get { return declaringType; }
	        set { declaringType = value; }
	    }

	    public MethodBase DeclaringMethod
	    {
	        get { return declaringMethod; }
	        set { declaringMethod = value; }
	    }

	    [DebuggerStepThrough]
		public void PreProcess(MarshalByRefObject target, ref IMethodCallMessage msg)
		{
			methodCallTarget=target;
			methodCallMessage=msg;
			methodReturnMessage = null;
			OnPreProcess();
		}

		[DebuggerStepThrough]
		protected abstract void OnPreProcess();
		[DebuggerStepThrough]
		protected abstract void OnPostProcess();


		[DebuggerStepThrough]
		public void PostProcess(MarshalByRefObject target,ref IMethodCallMessage msg,ref IMethodReturnMessage returnMessage)
		{
			methodCallTarget=target;
			methodCallMessage=msg;
			methodReturnMessage = returnMessage;
			OnPostProcess();
			//return message could have changed on the post processing
			//for example - swallowing an exception
			returnMessage = methodReturnMessage;

		}

		/// <summary>
		///     Executes the method that this attribute was declared on 
		///     with the same parameter values
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected void InvokeDeclaringMethod ()
		{
			methodCallMessage.MethodBase.Invoke(methodCallTarget,methodCallMessage.Args);
		}


	}
}