using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AnimationLib
{
	/// <summary>
	/// class for serializing animations
	/// </summary>
	public class AnimationXML
	{
		public string name = "";
		public float length = 0.0f;
		public List<KeyXML> keys = new List<KeyXML>();
	}
	
	/// <summary>
	/// this class is used to serialize animations to an xml file
	/// </summary>
	public class AnimationContainerXML
	{
		public List<AnimationXML> animations = new List<AnimationXML>();
	}
	
	/// <summary>
	/// this class is for serializing bone objects from an xml file
	/// </summary>
	public class BoneXML
	{
		public string name = "";
		public string type = "";
		public bool colorable = false;
		public List<JointXML> joints = new List<JointXML>();
		public List<ImageXML> images = new List<ImageXML>();
		public List<BoneXML> bones = new List<BoneXML>();
	}
	
	/// <summary>
	/// this class is for serializing bone objects from an xml file
	/// </summary>
	public class GarmentBoneXML : BoneXML
	{
		public string parentBone = "";
	}
	
	/// <summary>
	/// this class is for serializing image data from an xml file
	/// </summary>
	public class ImageXML
	{
		public Vector2 upperleft = new Vector2(0.0f);
		public Vector2 lowerright = new Vector2(0.0f);
		public Vector2 anchorcoord = new Vector2(0.0f);
		public string filename = "";
		public List<JointDataXML> joints = new List<JointDataXML>();
		public List<CircleXML> circles = new List<CircleXML>();
		public List<LineXML> lines = new List<LineXML>();
	}
	
	/// <summary>
	/// this class is for serializing joint data from an xml file
	/// </summary>
	public class JointXML
	{
		public string name = "";
	}
	
	public class GarmentXML
	{
		public string name = "";
		public List<GarmentFragmentXML> fragments = new List<GarmentFragmentXML>();
	}
	
	public class GarmentFragmentXML
	{
		public string model = "";
		public string animation = "";
	}
	
	public class JointDataXML
	{
		public Vector2 location = new Vector2(0.0f);
		public float limit1 = 0.0f;
		public float limit2 = 0.0f;
		public float FloatRadius = 0.0f;
		public bool FloatOrRotate = false;
	}
	
	public class KeyXML
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
	
	/// <summary>
	/// this class is used to serialize data from an XML file
	/// </summary>
	public class CircleXML
	{
		public Vector2 center = new Vector2(0.0f);
		public float radius = 0.0f;
	}
	
	public class LineXML
	{
		public Vector2 start = new Vector2(0.0f);
		public Vector2 end = new Vector2(0.0f);
	}
}