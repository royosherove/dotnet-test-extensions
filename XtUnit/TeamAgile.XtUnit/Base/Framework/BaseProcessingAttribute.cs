using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace TeamAgile.ApplicationBlocks.Interception
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
	/// 
	[Serializable]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property,AllowMultiple=true,Inherited=true)]
	public abstract class BaseProcessingAttribute : Attribute
	{

		protected void OutputDebugMessage (string message)
		{
			Debug.WriteLine(message);
			//Trace.WriteLine(message);
			//Console.WriteLine(message);
		}

		[DebuggerStepThrough]
		public void PreProcess(object sender,PreProcessEventArgs args)
		{
			OnPreProcess(sender,args);
		}

		[DebuggerStepThrough]
		protected abstract void OnPreProcess(object sender, PreProcessEventArgs args);
		[DebuggerStepThrough]
		protected abstract void OnPostProcess(object sender,PostProcessEventArgs args);


		/// <summary>
		///     Because PreProcess and PostProcess can instantiate new instances of your attribute
		///     you cannot keep local varibales in it to share between these two actions.
		///     use this method to save a setting that can be shared between those actions
		/// </summary>
		/// <param name="settingName" type="string">
		///     <para>
		///         the key name of the setting to be used later to get the value
		///     </para>
		/// </param>
		/// <param name="settingValue" type="object">
		///     <para>
		///         the value to be retrieved later
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		protected void SaveSettingForPostProcess (string settingName ,object settingValue)
		{
			string fullSettingName = buildFullSettingName (settingName);
			LocalDataStoreSlot slot = Thread.AllocateNamedDataSlot(fullSettingName);
			Thread.SetData(slot,settingValue);
		}

		private static string buildFullSettingName (string settingName)
		{
			return "XtUnit.AttributeSettings." + settingName;
		}

		/// <summary>
		///     use this method to retrieve data saved earlier using saveSettingForPostProcess method.
		/// </summary>
		/// <param name="settingName" type="string">
		///     <para>
		///         the key name of the setting
		///     </para>
		/// </param>
		/// <returns>
		///     A object value...
		/// </returns>
		protected object GetSettingFromPreProcess (string settingName)
		{
			string fullSettingName = buildFullSettingName (settingName);;
			LocalDataStoreSlot slot = Thread.GetNamedDataSlot(fullSettingName);
			return Thread.GetData(slot);
		}

		[DebuggerStepThrough]
		public void PostProcess(object sender, PostProcessEventArgs args)
		{
			OnPostProcess(sender,args);
			
			//return message could have changed on the post processing
			//for example - swallowing an exception
			//returnMessage = methodReturnMessage;
		}

		/// <summary>
		///     Executes the method that this attribute was declared on 
		///     with the same parameter values
		/// </summary>
		/// 
		/// <returns>
		///     A void value...
		/// </returns>
		protected void invokeDeclaringMethod (ProcessEventArgs args)
		{
			Invoker invoker =new Invoker();
			invoker.Invoke(args);
		}
		
		protected void FlagCurrentMethodToBeSkipped (ProcessEventArgs args)
		{
			IMethodCallMessage methodCallMessage = args.MethodCallMessage;

			ReturnMessage customMessage = new ReturnMessage(
				1,
				new object[]{}, 
				0, 
				methodCallMessage.LogicalCallContext, 
				methodCallMessage);
	
			methodCallMessage.LogicalCallContext.SetData("CustomReturnMessage",customMessage) ;
		}


	}

	[Serializable]
	class Invoker:MarshalByRefObject
	{

		public void Invoke (ProcessEventArgs args)
		{
			//Debug.WriteLine("** Invoke called.....");
			RemotingServices.ExecuteMessage(args.TargetObject,args.MethodCallMessage);
		}

		private Assembly domain_AssemblyResolve (object sender, ResolveEventArgs args)
		{
			string assemblyName = args.Name.Split(',')[1];
			string path = Path.GetDirectoryName( Assembly.GetExecutingAssembly ().FullName);
			string fullPath = Path.Combine(path,assemblyName);

			Debug.WriteLine("Resolving assembly name " + args.Name + " to :\n " + fullPath);

			return Assembly.LoadFrom(fullPath);
			
		}

	}

}