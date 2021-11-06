
namespace AnimationLib.Core.Json
{
	/// <summary>
	/// This is a peice of a garment that will get added to the skeleton
	/// </summary>
	public class GarmentFragmentJsonModel
	{
		/// <summary>
		/// The bone in the skeleton that this garment attaches to
		/// </summary>
		public string ParentBoneName { get; set; }

		/// <summary>
		/// Flag for whether or not this fragment completely covers other fragments underneath it.
		/// </summary>
		public bool DoesCover { get; set; }

		public string ModelFile { get; set; }

		public string AnimationFile { get; set; }
	}
}