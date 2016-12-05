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

	public class Config{
		private string _badConstant = "pristine";
		internal string BadConstant{
			get{
				return _badConstant;
			}
		}
	}

	[Test]
	public void CanManipulatePrivateParts() {
		var config = new Config();
		Assert.AreEqual( config.BadConstant, "pristine" );

		var field = typeof(Config).GetField("_badConstant", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		Assert.IsNotNull( field );
		Assert.AreEqual( field.GetValue(config), "pristine" );

		field.SetValue(config, "dirty" );
		Assert.AreNotEqual( config.BadConstant, "pristine" );
		Assert.AreEqual( config.BadConstant, "dirty" );
	}

	// Pro tips: Bury reflection's complexity inside tested classes to avoid regressions with Unity updates
	static class PlayerSettingsExtension {
		public static string cloudProjectId	{
			set {
				var property = typeof(PlayerSettings).GetProperty( "cloudProjectId" );
				var setMethod = property.GetSetMethod( true );
				setMethod.Invoke( null, new object[]{ value });
			}
		}
	}

	[Test]
	public void UnityKnowsWhatIsGoodForYou___OrNot(){
		Assert.AreEqual( PlayerSettings.cloudProjectId, "" );
		PlayerSettingsExtension.cloudProjectId = "12345";
		Assert.AreEqual( PlayerSettings.cloudProjectId, "12345" );
		PlayerSettingsExtension.cloudProjectId = "";
	}

	[Test]
	public void CanAccessAssembliesAsWell() {
		//var val = UnityEditorInternal.WebURLs.unity; // <-- this line does not compile
		var unaccessibleClass = Type.GetType("UnityEditorInternal.WebURLs,UnityEditor");
		Assert.IsNotNull( unaccessibleClass );

		var property = unaccessibleClass.GetProperty( "unity", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public );
		Assert.IsNotNull( property );
		var value = property.GetValue( null, new object[]{}) as string;
		Assert.AreEqual( value, "http://www.unity3d.com" );
	}
}
