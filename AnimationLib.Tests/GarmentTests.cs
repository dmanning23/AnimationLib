using AnimationLib;
using FilenameBuddy;
using NUnit.Framework;
using Shouldly;
using System;
using System.IO;

namespace Animationlib.Tests
{
	/// <summary>
	/// Tests for garments that attach to bones inside other garments,
	/// e.g. a gun barrel that attaches to a gun stock, so parts can be swapped out.
	/// </summary>
	[TestFixture()]
	public class GarmentTests
	{
		#region Setup

		private AnimationContainer _character;

		[SetUp]
		public void Setup()
		{
			Filename.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, "Content") + Path.DirectorySeparatorChar);

			_character = new AnimationContainer();
			var modelFile = new Filename();
			modelFile.SetRelFilename("Simple\\Simple Model.xml");
			_character.ReadSkeletonXml(modelFile, null);
		}

		private Garment LoadGarment(string name)
		{
			var file = new Filename();
			file.SetRelFilename($"Gun\\{name}.xml");
			return new Garment(file, _character.Skeleton, null);
		}

		private static GarmentBone RootGarmentBone(Garment garment)
		{
			return garment.Fragments[0].AnimationContainer.Skeleton.RootBone as GarmentBone;
		}

		private static Bone BarrelMount(Garment stock)
		{
			return stock.Fragments[0].AnimationContainer.Skeleton.RootBone.GetBone("BarrelMount");
		}

		#endregion //Setup

		#region Garment on garment

		[Test]
		public void CanLoadChildGarmentBeforeParentGarmentIsAdded()
		{
			//GameDonkey-style: load everything up front, add later
			var stock = LoadGarment("Wood Stock");
			Should.NotThrow(() => LoadGarment("Short Barrel"));
		}

		[Test]
		public void ChildGarmentAttachesToBoneInParentGarment()
		{
			var stock = LoadGarment("Wood Stock");
			var barrel = LoadGarment("Short Barrel");

			stock.AddToSkeleton();
			barrel.AddToSkeleton();

			RootGarmentBone(barrel).ParentBone.ShouldBeSameAs(BarrelMount(stock));
			BarrelMount(stock).ChildGarmentBone.ShouldBeSameAs(RootGarmentBone(barrel));
			_character.Skeleton.GetBone("Short Barrel").ShouldNotBeNull();
		}

		[Test]
		public void AddingChildGarmentBeforeParentGarmentThrowsHelpfulError()
		{
			var stock = LoadGarment("Wood Stock");
			var barrel = LoadGarment("Short Barrel");

			var ex = Should.Throw<Exception>(() => barrel.AddToSkeleton());
			ex.Message.ShouldContain("BarrelMount");
			ex.Message.ShouldContain("add that garment to the skeleton first");
		}

		[Test]
		public void SwappingParentGarmentMovesChildGarment()
		{
			var woodStock = LoadGarment("Wood Stock");
			var metalStock = LoadGarment("Metal Stock");
			var barrel = LoadGarment("Short Barrel");

			woodStock.AddToSkeleton();
			barrel.AddToSkeleton();

			//swap the stock
			woodStock.RemoveFromSkeleton();
			metalStock.AddToSkeleton();
			barrel.AddToSkeleton();

			RootGarmentBone(barrel).ParentBone.ShouldBeSameAs(BarrelMount(metalStock));
			BarrelMount(metalStock).ChildGarmentBone.ShouldBeSameAs(RootGarmentBone(barrel));
			BarrelMount(woodStock).ChildGarmentBone.ShouldBeNull();
			BarrelMount(woodStock).Bones.ShouldNotContain(RootGarmentBone(barrel));
		}

		[Test]
		public void SwappingChildGarmentReplacesIt()
		{
			var stock = LoadGarment("Wood Stock");
			var shortBarrel = LoadGarment("Short Barrel");
			var longBarrel = LoadGarment("Long Barrel");

			stock.AddToSkeleton();
			shortBarrel.AddToSkeleton();
			longBarrel.AddToSkeleton();

			BarrelMount(stock).ChildGarmentBone.ShouldBeSameAs(RootGarmentBone(longBarrel));
			shortBarrel.IsAdded.ShouldBeFalse();
			_character.Skeleton.GetBone("Short Barrel").ShouldBeNull();
			_character.Skeleton.GetBone("Long Barrel").ShouldNotBeNull();
		}

		[Test]
		public void ReAddingSameParentGarmentKeepsChildGarment()
		{
			var stock = LoadGarment("Wood Stock");
			var barrel = LoadGarment("Short Barrel");

			stock.AddToSkeleton();
			barrel.AddToSkeleton();
			stock.RemoveFromSkeleton();
			stock.AddToSkeleton();

			_character.Skeleton.GetBone("Short Barrel").ShouldNotBeNull();
		}

		#endregion //Garment on garment
	}
}
