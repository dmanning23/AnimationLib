using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// class for sorting the keyframe XML by time
	/// </summary>
	class KeyElementModelSort : IComparer<KeyElementModel>
	{
		public int Compare(KeyElementModel key1, KeyElementModel key2)
		{
			return key1.Time.CompareTo(key2.Time);
		}
	}

	/// <summary>
	/// This class is for reading key elements from an animation file
	/// </summary>
	public class KeyElementModel : XmlObject
	{
		#region Properties

		public int Time { get; set; }
		public float Rotation { get; set; }
		public int Layer { get; set; }
		public string Image { get; set; }
		public bool Flip { get; set; }
		public Vector2 Translation { get; set; }
		public bool Ragdoll { get; set; }
		public string Joint { get; set; }

		#endregion //Properties

		#region Methods

		public KeyElementModel()
		{
			Flip = false;
			Translation = Vector2.Zero;
			Ragdoll = false;
		}

		public KeyElementModel(KeyElement key, Bone bone)
		{
			Debug.Assert(null != bone);

			//find the image this key element uses
			if (-1 != key.ImageIndex)
			{
				//find the bone that uses this dudes joint as a keyjoint
				var childBone = bone.GetBone(key.BoneName);
				if (null != childBone)
				{
					Debug.Assert(key.ImageIndex < childBone.Images.Count);
					Image = childBone.Images[key.ImageIndex].Name;
				}
			}

			//create the xml object and add it to the animation
			Flip = key.Flip;
			Joint = key.BoneName;
			Layer = key.Layer;
			Ragdoll = key.Ragdoll;

			//Set the rotation to 0 if this dude is using ragdoll
			if (!Ragdoll)
			{
				Rotation = key.Rotation;
			}
			Time = key.Time;
			Translation = key.Translation;
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			var name = node.Name;
			var value = node.InnerText;

			switch (name)
			{
				case "Type":
					{
						//throw these attributes out
					}
					break;
				case "time":
					{
						Time = Convert.ToInt32(value);
					}
					break;
				case "rotation":
					{
						Rotation = MathHelper.ToRadians(Convert.ToSingle(value));
					}
					break;
				case "layer":
					{
						Layer = Convert.ToInt32(value) * -1;
					}
					break;
				case "image":
					{
						Image = value;
					}
					break;
				case "flip":
					{
						Flip = Convert.ToBoolean(value);
					}
					break;
				case "translation":
					{
						Translation = value.ToVector2();
					}
					break;
				case "ragdoll":
					{
						Ragdoll = Convert.ToBoolean(value);
					}
					break;
				case "joint":
					{
						Joint = value;
					}
					break;
				default:
					{
						base.ParseXmlNode(node);
					}
					break;
			}
		}

#if !WINDOWS_UWP
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("key");

			xmlWriter.WriteAttributeString("time", Time.ToString());
			xmlWriter.WriteAttributeString("joint", Joint);

			if (!SkipRotation)
			{
				xmlWriter.WriteAttributeString("rotation", MathHelper.ToDegrees(Rotation).ToString());
			}

			if (!SkipLayer)
			{
				var layer = Layer * -1;
				xmlWriter.WriteAttributeString("layer", layer.ToString());
			}

			if (!SkipImage)
			{
				xmlWriter.WriteAttributeString("image", Image);
			}

			if (!SkipFlip)
			{
				xmlWriter.WriteAttributeString("flip", Flip ? "true" : "false");
			}

			if (!SkipTranslation)
			{
				xmlWriter.WriteAttributeString("translation", Translation.StringFromVector());
			}

			if (!SkipRagDoll)
			{
				xmlWriter.WriteAttributeString("ragdoll", Ragdoll ? "true" : "false");
			}

			xmlWriter.WriteEndElement();
		}
#endif

		public bool SkipRotation
		{
			get
			{
				return 0.0f == Rotation;
			}
		}

		public bool SkipLayer
		{
			get
			{
				return 0 == Layer;
			}
		}

		public bool SkipImage
		{
			get
			{
				return string.IsNullOrEmpty(Image);
			}
		}

		public bool SkipFlip
		{
			get
			{
				return false == Flip;
			}
		}

		public bool SkipTranslation
		{
			get
			{
				return Vector2.Zero == Translation;
			}
		}

		public bool SkipRagDoll
		{
			get
			{
				return false == Ragdoll;
			}
		}

		#endregion //File IO
	}
}