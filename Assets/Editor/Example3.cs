using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;

namespace TestNamespace{
	static class HiddenClass
	{
		static internal string Foo(){
			return "Bar";
		}

	}
	public static class Config{
		private static string _badConstant = "pristine";
		internal static string BadConstant{
			get{
				return _badConstant;
			}
		}
	}
}


public class Ex3_Voyeurism {
	
	[Test]
	public void CanCallInternalMethods() {
		Assert.NotNull(Type.GetType( "TestNamespace.HiddenClass" ));
		Assert.IsNull(Type.GetType( "TestNamespace.HiddenClass" ).GetMethod("Foo"));
		Assert.IsNotNull(Type.GetType( "TestNamespace.HiddenClass" ).GetMethod("Foo", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Static));
		var method = Type.GetType( "TestNamespace.HiddenClass" ).GetMethod("Foo", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Static);
		Assert.AreEqual(method.Invoke(null, new object[]{}), "Bar");
	}

	[Test]
	public void CanManipulatePrivateParts() {
		Assert.AreEqual( TestNamespace.Config.BadConstant, "pristine" );
		var field = typeof(TestNamespace.Config).GetField("_badConstant", System.Reflection.BindingFlags.NonPublic| System.Reflection.BindingFlags.Static);
		Assert.AreEqual( field.GetValue(null), "pristine" );
		field.SetValue(null, "dirty" );
		Assert.AreNotEqual( TestNamespace.Config.BadConstant, "pristine" );
		Assert.AreEqual( TestNamespace.Config.BadConstant, "dirty" );
	}
}
