#define DEFINED

using NUnit.Framework;
using System.Diagnostics;
using System;

public class Example5_Conditional {

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
