using FilenameBuddy;
using System;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class GarmentSkeleton : Skeleton
	{
		#region Properties

		public Garment Garment { get; private set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentSkeleton(GarmentAnimationContainer animations, Garment garment)
			: base(animations)
		{
			Garment = garment;
		}

		#endregion //Methods

		#region File IO

		protected override Bone CreateBone(SkeletonModel skeleton)
		{
			var garmentBone = skeleton.RootBone as GarmentBoneModel;
			if (null == garmentBone)
			{
				throw new Exception("wrong bone type passed to GarmentSkeleton.CreateBone");
			}
			return new GarmentBone(Animations as GarmentAnimationContainer, garmentBone, Garment);
		}

		public void WriteXml(Filename filename, GarmentFragmentModel fragment)
		{
			var skeleton = new GarmentSkeletonModel(filename, this, fragment);
			skeleton.WriteXml();
		}

		#endregion //Model File IO
	}
}