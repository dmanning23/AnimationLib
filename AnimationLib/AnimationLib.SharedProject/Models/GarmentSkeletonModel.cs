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
		public GarmentSkeletonModel(Filename filename, float scale, GarmentFragmentModel fragment)
			: base(filename, scale, fragment.FragmentScale)
		{
			RootBone = new GarmentBoneModel(this, scale, fragment);
		}

		public GarmentSkeletonModel(Filename filename, GarmentSkeleton skeleton, GarmentFragmentModel fragment)
			: base(filename, 1f, fragment.FragmentScale)
		{
			RootBone = new GarmentBoneModel(this, skeleton.RootBone as GarmentBone, fragment);
		}

		#endregion //Methods
	}
}