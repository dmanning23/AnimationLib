using FilenameBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class GarmentSkeletonModel : SkeletonModel
	{
		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentSkeletonModel(Filename filename, float scale, float fragmentScale)
			: base(filename, scale, fragmentScale)
		{
			RootBone = new GarmentBoneModel(scale, fragmentScale);
		}

		public GarmentSkeletonModel(Filename filename, GarmentSkeleton skeleton)
			: base(filename, 1f, 1f)
		{
			RootBone = new GarmentBoneModel(skeleton.RootBone as GarmentBone);
		}

		#endregion //Methods
	}
}