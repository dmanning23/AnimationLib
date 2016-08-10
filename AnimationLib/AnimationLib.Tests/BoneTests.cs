using NUnit.Framework;
using System;
using AnimationLib;
using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;

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

		#region load tests

		[Test()]
		public void LoadedCrotch()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.IsNotNull(test.Skeleton.RootBone);
		}

		[Test()]
		public void LoadedCrotchName()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual("Crotch", test.Skeleton.RootBone.Name);
		}

		[Test()]
		public void LoadedCrotchType()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(EBoneType.Normal, test.Skeleton.RootBone.BoneType);
		}

		[Test()]
		public void LoadedCrotchColorable()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(false, test.Skeleton.RootBone.Colorable);
		}

		[Test()]
		public void LoadedCrotchJoints()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(1, test.Skeleton.RootBone.Joints.Count);
		}

		[Test()]
		public void LoadedCrotchImages()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(3, test.Skeleton.RootBone.Images.Count);
		}

		[Test()]
		public void LoadedCrotchBones()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(1, test.Skeleton.RootBone.Bones.Count);
		}

		[Test()]
		public void LoadedTorso()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.IsNotNull(test.Skeleton.RootBone.GetBone("Torso"));
		}

		[Test()]
		public void LoadedTorsoType()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(EBoneType.Foot, test.Skeleton.RootBone.GetBone("Torso").BoneType);
		}

		[Test()]
		public void LoadedTorsoColorable()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(true, test.Skeleton.RootBone.GetBone("Torso").Colorable);
		}

		[Test()]
		public void LoadedChesticle()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.IsNotNull(test.Skeleton.RootBone.GetBone("Chesticle"));
		}

		[Test()]
		public void LoadedChesticleType()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);

			Assert.AreEqual(EBoneType.Weapon, test.Skeleton.RootBone.GetBone("Chesticle").BoneType);
		}

		#endregion //load tests

		#region Update tests

		[Test()]
		public void LoadedAnimation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 1.0f, 0.0f, true);

			Assert.AreEqual(Vector2.Zero, test.Skeleton.RootBone.AnchorPosition);
		}

		[Test()]
		public void CrotchLocation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 1.0f, 0.0f, true);

			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");

			Assert.AreEqual(Vector2.Zero, crotch.AnchorPosition);
		}

		[Test()]
		public void jointlocation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 1.0f, 0.0f, true);
			Bone torso = test.Skeleton.RootBone.GetBone("Torso");
			Assert.AreEqual(new Vector2(-7.0f, -18.0f), torso.AnchorPosition);
		}

		[Test()]
		public void CrotchImageLocation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 1.0f, 0.0f, true);
			Bone Crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Assert.AreEqual(new Vector2(-74.0f, -62.0f), Crotch.Position);
		}

		[Test()]
		public void TorsoImageLocation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 1.0f, 0.0f, true);
			Bone Crotch = test.Skeleton.RootBone.GetBone("Torso");
			Assert.AreEqual(new Vector2(-44.0f, -117.0f), Crotch.Position);
		}

		[Test()]
		public void TorsoImageLocation1()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			GameClock timer = new GameClock();
			test.Update(timer, new Vector2(200.0f, 300.0f), false, 1.0f, 0.0f, true);
			Bone Crotch = test.Skeleton.RootBone.GetBone("Torso");
			Assert.AreEqual(new Vector2(156.0f, 183.0f), Crotch.Position);
		}

		#endregion //Update tests
	}
}
