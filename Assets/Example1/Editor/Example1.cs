using NUnit.Framework;
using System;
using System.Reflection;

public class Ex1_WikipediaExample {
	class Foo{
		public bool Invoked{ get; private set; }
		public void hello() {
			Invoked = true;
		}
	}
	[Test]
	public void CanDoSomeStuff() {
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

		Assert.IsInstanceOf<Foo>( instance );
		Assert.IsTrue(((Foo)instance).Invoked);
	}
}
