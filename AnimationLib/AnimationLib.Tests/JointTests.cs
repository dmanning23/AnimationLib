using AnimationLib;
using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Animationlib.Tests
{
	[TestFixture]
	public class JointTests
	{
		#region Setup

		[SetUp]
		public void Setup()
		{
			Filename.SetCurrentDirectory(@"C:\Projects\animationlib\AnimationLib\AnimationLib.Tests\Content\");
		}

		#endregion //Setup

		#region Tests

		[Test]
		public void Loaded()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);

			Joint torso = test.Skeleton.RootBone.GetJoint("Torso");
			Assert.NotNull(torso);
		}

		[Test]
		public void LoadLocation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			Joint torso = test.Skeleton.RootBone.GetJoint("Torso");
			Assert.AreEqual(0.0f, torso.Position.X);
			Assert.AreEqual(0.0f, torso.Position.Y);
		}

		[Test]
		public void UpdateLocation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 0.0f, true);
			Joint torso = test.Skeleton.RootBone.GetJoint("Torso");
			Assert.AreEqual(new Vector2(-7.0f, -18.0f), torso.Position);
		}

		[Test]
		public void UpdateLocation1()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 0.0f, true);
			Joint torso = test.Skeleton.RootBone.GetJoint("Torso");
			Assert.NotNull(torso.Data);
		}

		[Test]
		public void UpdateLocation2()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
			test.SetAnimation(0, EPlayback.Forwards);

			GameClock timer = new GameClock();
			test.Update(timer, Vector2.Zero, false, 0.0f, true);
			Joint torso = test.Skeleton.RootBone.GetJoint("Torso");
			Assert.AreEqual(67.0f, torso.Data.Location.X);
			Assert.AreEqual(44.0f, torso.Data.Location.Y);
		}

		#endregion //tests
	}
}
