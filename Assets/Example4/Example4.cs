using UnityEngine.Assertions;

public class Example4 : MonobehaviourWithPrivateAwake {
	public Example4Data Data = null;

	public void Awake() {
		Assert.AreEqual( MemberToBeOverriden, "Foo" );
		base.MemberToBeOverriden = Data.OverrideData;
		Assert.AreEqual( MemberToBeOverriden, "Bar" );

		var baseAwake = typeof(MonobehaviourWithPrivateAwake).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		baseAwake.Invoke( this, new object[]{} );

		IntegrationTest.Pass();
	}
}
