using NUnit.Framework;
using System;
using AnimationLib;
using FilenameBuddy;

namespace Animationlib.Tests
{
	[TestFixture()]
	public class AnimationContainerTests
	{
		#region load tests

		[Test()]
		public void LoadedModel()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			bool bTest = test.ReadXMLModelFormat(testFile.File, null);
			Assert.IsTrue(bTest);
		}

		[Test()]
		public void LoadedAnimation()
		{
			AnimationContainer test = new AnimationContainer();
			Filename testFile = new Filename();
			testFile.SetRelFilename("Simple\\Simple Model.xml");
			test.ReadXMLModelFormat(testFile.File, null);
			testFile.SetRelFilename("Simple\\Simple Animations.xml");
			bool bTest = test.ReadXMLAnimationFormat(testFile.File);
			Assert.IsTrue(bTest);
		}

		#endregion //load tests
	}
}

