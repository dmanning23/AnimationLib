using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AnimationLib
{
	/// <summary>
	/// class for serializing animations
	/// </summary>
	public class AnimationXml
	{
		public string name = "";
		public float length = 0.0f;
		public List<KeyXml> keys = new List<KeyXml>();
	}

	public class KeyXml
	{
		public int time = 0;
		public float rotation = 0.0f;
		public int layer = 0;
		public string image = "";
		public bool flip = false;
		public Vector2 translation = new Vector2(0.0f);
		public bool ragdoll = false;
		public string joint = "";

		public bool SkipRotation
		{
			get
			{
				return 0.0f == rotation;
			}
		}
		public bool SkipLayer
		{
			get
			{
				return 0 == layer;
			}
		}
		public bool SkipImage
		{
			get
			{
				return string.IsNullOrEmpty(image);
			}
		}
		public bool SkipFlip
		{
			get
			{
				return false == flip;
			}
		}
		public bool SkipTranslation
		{
			get
			{
				return Vector2.Zero == translation;
			}
		}
		public bool SkipRagDoll
		{
			get
			{
				return false == ragdoll;
			}
		}
	}
}