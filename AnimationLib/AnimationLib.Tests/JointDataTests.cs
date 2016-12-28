using AnimationLib;
using FilenameBuddy;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Animationlib.Tests
{
	[TestFixture]
	public class JointDataTests
	{
		Image image;

		[SetUp]
		public void Setup()
		{
			Filename.SetCurrentDirectory(@"C:\Projects\animationlib\AnimationLib\AnimationLib.Tests\Content\");

			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			image = crotch.GetImage(file);
		}

		[Test]
		public void Loaded()
		{
			Assert.AreEqual(1, image.JointCoords.Count);
		}

		[Test]
		public void Location()
		{
			Assert.AreEqual(new Vector2(67.0f, 44.0f), image.JointCoords[0].Location);
		}

		[Test]
		public void Limit1()
		{
			Assert.AreEqual(-180.0f, MathHelper.ToDegrees(image.JointCoords[0].FirstLimit));
		}

		[Test]
		public void Limit2()
		{
			Assert.AreEqual(260.0f, MathHelper.ToDegrees(image.JointCoords[0].SecondLimit));
		}

		[Test]
		public void Limit1Flip()
		{
			Assert.AreEqual(-260.0f, MathHelper.ToDegrees(image.JointCoords[0].FirstLimitFlipped));
		}

		[Test]
		public void Limit2Flip()
		{
			Assert.AreEqual(180.0f, MathHelper.ToDegrees(image.JointCoords[0].SecondLimitFlipped));
		}

		[Test]
		public void DeafultRagdollType()
		{
			Assert.AreEqual(RagdollType.Float, image.JointCoords[0].RagdollType);
		}

		[Test]
		public void Length()
		{
			Assert.AreEqual(10.0f, image.JointCoords[0].FloatRadius);
		}
	}
}

