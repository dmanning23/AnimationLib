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
		public GarmentSkeletonModel(Filename filename)
			: base(filename)
		{
			RootBone = new GarmentBoneModel();
		}

		public GarmentSkeletonModel(Filename filename, GarmentSkeleton skeleton)
			: base(filename)
		{
			RootBone = new GarmentBoneModel(skeleton.RootBone as GarmentBone);
		}

		#endregion //Methods
	}
}