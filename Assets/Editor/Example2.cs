using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;

public class Example2 {

	interface IHandler{
		string Handle(string message);
	}

	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple=true)]
	sealed class Handle: System.Attribute {
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
			return "Null: " + message;
		}
	}


	Dictionary<string, System.Type> messageTypesToHandler = new Dictionary<string, System.Type>();

	void CreateCache() {
		// Do search for these private types only in this class
		foreach ( var type in typeof( Example2 ).GetNestedTypes(BindingFlags.NonPublic) ) {
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

	[Test]
	public void EditorTest() {
		CreateCache();

		Assert.AreEqual( GetHandler("Foo").Handle( "This message is for foo" ), "Foo: This message is for foo" );
		Assert.AreEqual( GetHandler("Bar").Handle( "This message is for bar" ), "Bar: This message is for bar" );
		Assert.AreEqual( GetHandler("Bar2").Handle( "This message is for bar" ), "Bar: This message is for bar" );
		Assert.AreEqual( GetHandler("Unknown handler").Handle( "DERP" ), "Null: DERP" );
		Assert.AreEqual( GetHandler("Invalid").Handle( "Nope" ), "Null: Nope" );
	}
}
