using System.Collections.Generic;

namespace AnimationLib.Core.Json
{
	/// <summary>
	/// This object is a single animation for the skeleton
	/// </summary>
	public class AnimationJsonModel
	{
		/// <summary>
		/// name of the animation
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// length of this animation
		/// </summary>
		/// <value>The length.</value>
		public float Length { get; set; }

		/// <summary>
		/// All the key elements for this animation
		/// </summary>
		public List<KeyElementJsonModel> KeyElements { get; set; }
	}
}