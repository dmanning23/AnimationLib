using AnimationLib;
using FilenameBuddy;
using NUnit.Framework;

namespace Animationlib.Tests
{
	[TestFixture]
	public class AnimationContainerTests
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
		public void LoadedModel()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
		}

		[Test]
		public void LoadedAnimation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadSkeletonXml(testFile, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			test.ReadAnimationXml(testFile);
		}

		#endregion //load tests
	}
}

