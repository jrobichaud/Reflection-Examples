#define DEFINED

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

public class Ex2_Factory {

	interface IHandler{
		string Handle(string message);
	}

	[System.AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	sealed class Handle: Attribute {
		public string MessageType{get;set;}
		public Handle( string messageType ) {
			MessageType = messageType;		
		}
	}

	[Handle("Foo")]
	class FooHandler : IHandler{
		public string Handle( string message ) {
			return "Foo: " + message;
		}		
	}

	[Handle("Bar")]
	[Handle("Bar2")]
	class Handler : IHandler{
		public string Handle( string message ) {
			return "Bar: " + message;
		}
	}

	[Handle("Invalid")]
	class InvalidHandler { // : IHandler{ <-- this is intentional
		public string Handle( string message ) {
			throw new ExecutionEngineException();
		}
	}

	class NullHandler : IHandler{
		public string Handle( string message ) {
			return "Nope";
		}
	}


	Dictionary<string, Type> messageTypesToHandler = new Dictionary<string, Type>();

	void CreateCache() {
		// Do search for these private types only in this class
		foreach ( var type in typeof( Ex2_Factory ).GetNestedTypes(BindingFlags.NonPublic) ) {
			if ( typeof(IHandler).IsAssignableFrom( type ) ) {
				foreach ( Handle handle in type.GetCustomAttributes( typeof( Handle ), false ) ) {
					messageTypesToHandler.Add( handle.MessageType, type );
				}
			}
		}
	}

	IHandler GetHandler(string messageType){
		Type handlerType = null;
		if ( messageTypesToHandler.TryGetValue(messageType, out handlerType ) )
			return Activator.CreateInstance( handlerType ) as IHandler;
		else
			return new NullHandler();
	}
	public Ex2_Factory(){
		CreateCache();
	}
	[Test]
	public void DispatchMessagesCorretly() {
		Assert.AreEqual( GetHandler("Foo").Handle( "This message is for foo" ), "Foo: This message is for foo" );
		Assert.AreEqual( GetHandler("Bar").Handle( "This message is for bar" ), "Bar: This message is for bar" );
		Assert.AreEqual( GetHandler("Bar2").Handle( "This message is for bar" ), "Bar: This message is for bar" );
	}

	[Test]
	public void FailsGracefullyWithBadData(){
		Assert.DoesNotThrow( ()=> GetHandler("jkhfsgjhsdjghjdshgjhdgjh").Handle("ajsgjasjgdhjhgjdhfgjdhgj") );
	}

	[Test]
	public void DoesNotDispatchToClassThatDoesNotInheritFromIHandler(){
		Assert.IsInstanceOf<NullHandler>( GetHandler("Invalid") );
	}

}


public class Ex2_ConditionalCompilation
{
	[Conditional("NOT_DEFINED")]
	void NotDefinedMethod(){
		throw new Exception();
	}

	[Conditional("DEFINED")]
	void DefinedMethod(){
		throw new Exception();
	}

	[Test]
	public void NotDefinedIsNotCalled() {
		Assert.DoesNotThrow( ()=>{
			NotDefinedMethod();
		});
	}

	[Test]
	public void DefinedIsCalled() {
		Assert.Throws<Exception>( ()=>{
			DefinedMethod();
		});
	}
}
