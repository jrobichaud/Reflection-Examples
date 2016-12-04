using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class Example1 : MonoBehaviour {

	class Foo{
		public void hello() {
			Debug.Log("Hello");
		}
	}

	// Use this for initialization
	void Start () {
		//https://fr.wikipedia.org/wiki/R%C3%A9flexion_(informatique)#Exemple
		// Sans utiliser la réflexion
		Foo foo = new Foo();
		foo.hello();

		// En utilisant la réflexion
		Type type = typeof(Foo);
		// Instanciation de l'objet dont la méthode est à appeler
		// Il existe deux façons d'instancier un objet via la réflexion :
		// 1, soit :
		ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
		object instance = constructor.Invoke(new object[]{});
		// 2, soit :
		instance = Activator.CreateInstance(type);

			// Invocation de la méthode via réflexion
		MethodInfo method = type.GetMethod("hello");
		method.Invoke(instance, new object[]{});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
