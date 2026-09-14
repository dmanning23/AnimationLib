using AnimationLib.Core.Json;
using FilenameBuddy;
using Microsoft.Xna.Framework.Content;

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

		public override void ReadJsonFile(ContentManager content = null)
		{
			using (var jsonModel = new SkeletonJsonModel(this.ContentName, this.Filename))
			{
				//read the json file
				jsonModel.ReadJsonFile(content);

				//load from the json structure
				var garmentBone = RootBone as GarmentBoneModel;
				var scale = garmentBone.Scale;
				var fragmentScale = garmentBone.FragmentScale;
				var fragment = garmentBone.FragmentModel;
				RootBone = new GarmentBoneModel(this, jsonModel.RootBone, scale, fragmentScale, fragment);
			}
		}

		#endregion //Methods
	}
}