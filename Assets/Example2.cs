using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class Example2 : MonoBehaviour {


	interface IHandler{
		void Handle(string message);
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
		public void Handle( string message ) {
			Debug.Log( "Foo: " + message );
		}		
	}

	[Handle("Bar")]
	[Handle("Bar2")]
	class Handler : IHandler{
		public void Handle( string message ) {
			Debug.Log( "Bar: " + message );
		}
	}

	[Handle("Invalid")]
	class InvalidHandler {
		public void Handle( string message ) {
			Debug.Log( "Bar: " + message );
		}
	}

	class NullHandler : IHandler{
		public void Handle( string message ) {
			Debug.Log( "Null handler: " + message );
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

	// Use this for initialization
	void Start () {
		CreateCache();
		GetHandler("Foo").Handle( "This message is for foo" );
		GetHandler("Bar").Handle( "This message is for bar" );
		GetHandler("Bar2").Handle( "This message is for bar as well" );
		GetHandler("Invalid handler").Handle( "DERP" );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
