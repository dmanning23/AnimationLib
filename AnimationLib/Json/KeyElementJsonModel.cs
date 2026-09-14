using Microsoft.Xna.Framework;

namespace AnimationLib.Core.Json
{
	/// <summary>
	/// This class is for reading key elements from an animation file
	/// </summary>
	public class KeyElementJsonModel
	{
		public int Time { get; set; }
		public float Rotation { get; set; }
		public int Layer { get; set; }
		public string Image { get; set; }
		public bool Flip { get; set; }
		public Vector2 Translation { get; set; }
		public bool Ragdoll { get; set; }
		public string Joint { get; set; }
	}
}