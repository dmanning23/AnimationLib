
using Microsoft.Xna.Framework;

namespace AnimationLib.Core.Json
{
	public class PhysicsLineJsonModel
	{
		/// <summary>
		/// the start of the line, local to the bone
		/// </summary>
		public Vector2 Start { get; set; }

		/// <summary>
		/// the end of the line, local to the bone
		/// </summary>
		public Vector2 End { get; set; }
	}
}