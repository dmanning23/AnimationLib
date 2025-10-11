using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace AnimationLib.Core.Json
{
	/// <summary>
	/// This is the model for a bone as laoded from a file, data store, etc
	/// </summary>
	public class BoneJsonModel
	{
		/// <summary>
		/// The name of this bone
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Flag used to tell the difference between the bone types for collision purposes
		/// </summary>
		public EBoneType BoneType { get; set; } = EBoneType.Normal;

		/// <summary>
		/// Whether or not this bone should be colored by the palette swap
		/// </summary>
		public bool Colorable { get; set; }

		public float RagdollWeightRatio { get; set; } = 0.5f;

		public string PrimaryColorTag { get; set; }
		public string SecondaryColorTag { get; set; }

		/// <summary>
		/// the child bones of this guy
		/// </summary>
		public List<BoneJsonModel> Bones { get; set; }

		/// <summary>
		/// The joints in this bone
		/// </summary>
		public List<JointJsonModel> Joints { get; set; }

		/// <summary>
		/// The images in this bone
		/// </summary>
		public List<ImageJsonModel> Images { get; set; }
	}
}