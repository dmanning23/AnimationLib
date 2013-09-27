using NUnit.Framework;
using System;
using AnimationLib;

namespace Animationlib.Tests
{
	[TestFixture()]
	public class BoneTests
	{
		#region Type Tests

		[Test()]
		public void WeaponTypeTest()
		{
			Bone test = new Bone();
			test.BoneType = EBoneType.Weapon;
			Assert.IsTrue(test.IsWeapon);
		}

		[Test()]
		public void FootTypeTest()
		{
			Bone test = new Bone();
			test.BoneType = EBoneType.Foot;
			Assert.IsTrue(test.IsFoot);
		}

		[Test()]
		public void TestDefaultType()
		{
			Bone test = new Bone();
			Assert.IsFalse(test.IsWeapon);
			Assert.IsFalse(test.IsFoot);
			Assert.AreEqual(EBoneType.Normal, test.BoneType);
		}

		#endregion //Type Tests
	}
}

