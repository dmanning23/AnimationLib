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
			test.ReadSkeletonXml(testFile, null);
		}

		[Test()]
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

