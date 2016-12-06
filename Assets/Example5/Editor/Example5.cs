using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using System.Reflection;

public class Example5_Deconstructor {

	sealed class Foo{
		public Foo(){
			throw new Exception();
		}
		public void Bar(){
			Assert.Pass();
		}
	}

	[Test]
	public void CallBarWithoutException(){

		Assert.Throws<Exception>( ()=> new Foo() );

		Assert.Throws<SuccessException>( ()=> {
			var instance = (Foo)FormatterServices.GetUninitializedObject(typeof(Foo)); 
			instance.Bar();
		} );
	}
}


public class Example5_Precondition {

	sealed class Foo{
		public bool ShouldThrow = true;
		public Foo(){
			if ( ShouldThrow ){
				throw new Exception();
			}
		}
	}

	[Test]
	public void CallBarWithoutException(){

		Assert.Throws<Exception>( ()=> new Foo() );
		Assert.Throws<Exception>( ()=> new Foo{ShouldThrow=false} );

		Assert.Throws<TargetInvocationException>( ()=> {
			var instance = (Foo)FormatterServices.GetUninitializedObject(typeof(Foo)); 
			instance.ShouldThrow = false;
			var constructor = typeof(Foo).GetConstructor(Type.EmptyTypes);
			constructor.Invoke( instance, new object[]{} );
		} );
	}

}