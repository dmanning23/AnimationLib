using AnimationLib;
using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace Animationlib.Tests
{
	[TestFixture]
	public class ImageTests
	{
		#region Setup

		[SetUp]
		public void Setup()
		{
			Filename.SetCurrentDirectory(@"C:\Projects\animationlib\AnimationLib\AnimationLib.Tests\Content\");
		}

		#endregion //Setup

		#region load tests

		[Test]
		public void LoadFile()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			////TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
		}

		[Test]
		public void GotBone()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			////TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");

			Assert.IsNotNull(crotch);
		}

		[Test]
		public void GotImage()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			////TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");

			Assert.IsNotEmpty(crotch.Images);
		}

		[Test]
		public void GotImage1()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");

			Assert.AreEqual(3, crotch.Images.Count);
		}

		[Test]
		public void GetImageIndex()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");

			Assert.AreEqual(0, crotch.GetImageIndex("CrotchFront.png"));
		}

		[Test]
		public void GetImage()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");

			Assert.IsNotNull(crotch.GetImage(file));
		}

		#endregion //load tests

		#region coord tests

		[Test]
		public void GetAnchorCoord()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			Image image = crotch.GetImage(file);

			Assert.AreEqual(new Vector2(74.0f, 62.0f), image.AnchorCoord);
		}

		[Test]
		public void GetUL_UV()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			Image image = crotch.GetImage(file);

			Assert.AreEqual(Vector2.Zero, image.UpperLeft);
		}

		[Test]
		public void SetGetLR_UV()
		{
			Vector2 test = new Vector2(137.0f, 150.0f);
			Image image = new Image();
			image.LowerRight = test;

			Assert.AreEqual(test, image.LowerRight);
		}

		[Test]
		public void GetWidth()
		{
			Vector2 test = new Vector2(137.0f, 150.0f);
			Image image = new Image();
			image.LowerRight = test;

			Assert.AreEqual(137.0f, image.Width);
		}

		[Test]
		public void GetLR_UV()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			Image image = crotch.GetImage(file);

			Assert.AreEqual(new Vector2(129.0f, 116.0f), image.LowerRight);
		}

		[Test]
		public void GetWidth1()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			Image image = crotch.GetImage(file);

			Assert.AreEqual(129.0f, image.Width);
		}

		[Test]
		public void GetFlippedAnchorCoord()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			Image image = crotch.GetImage(file);

			Assert.AreEqual(new Vector2(74.0f, 62.0f), image.GetFlippedAnchorCoord(false, 1.0f));
		}

		[Test]
		public void GetFlippedAnchorCoord1()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			Image image = crotch.GetImage(file);

			Assert.AreEqual(new Vector2(55.0f, 62.0f), image.GetFlippedAnchorCoord(true, 1.0f));
		}

		[Test]
		public void GetFlippedAnchorCoord2()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("SuperSimple\\Simple Model.xml");
			//TestRenderer renderer = new TestRenderer();
			test.ReadSkeletonXml(testFile, null);
			Bone crotch = test.Skeleton.RootBone.GetBone("Crotch");
			Filename file = new Filename();
			file.SetRelFilename(@"SuperSimple\CrotchFront.png");
			Image image = crotch.GetImage(file);

			Assert.AreEqual(new Vector2(110.0f, 124.0f), image.GetFlippedAnchorCoord(true, 2.0f));
		}

		#endregion //coord tests

		#region Update Tests

		[Test]
		public void UpdateIndex()
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
			Assert.AreEqual(0, test.Skeleton.RootBone.ImageIndex);
		}

		[Test]
		public void UpdateImage()
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
			Assert.NotNull(test.Skeleton.RootBone.GetCurrentImage());
		}

		#endregion //Update Tests
	}
}
