using UnityEngine;
using UnityEngine.Assertions;


public class MonobehaviourWithPrivateAwake : MonoBehaviour {

	public string MemberToBeOverriden = "Foo";

	// This method is private and cannot be overriden
	void Awake () {
		Assert.AreEqual( MemberToBeOverriden, "Bar" );
	}
}
