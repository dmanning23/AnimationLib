using AnimationLib.Core.Json;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
#if !BRIDGE
using System.Xml;
#endif
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
		private float Scale { get; set; }

		#endregion //Properties

		#region Methods

		public KeyElementModel(float scale)
		{
			Scale = scale;
			Flip = false;
			Translation = Vector2.Zero;
			Ragdoll = false;
		}

		public KeyElementModel(KeyElementJsonModel key, float scale) : this(scale)
		{
			Time = key.Time;
			Rotation = key.Rotation;
			Layer = key.Layer;
			Image = key.Image;
			Flip = key.Flip;
			Translation = key.Translation;
			Ragdoll = key.Ragdoll;
			Joint = key.Joint;
		}

		public KeyElementModel(KeyElement key, Bone bone) : this(1f)
		{
			//find the image this key element uses
			if (-1 != key.ImageIndex)
			{
				//find the bone that uses this dudes joint as a keyjoint
				var childBone = bone.GetBone(key.BoneName);
				if (null != childBone && childBone.Images.Count > key.ImageIndex)
				{
					Image = childBone.Images[key.ImageIndex].Name;
				}
			}

			//create the xml object and add it to the animation
			Flip = key.Flip;
			Joint = key.BoneName;
			Layer = key.Layer;
			Ragdoll = key.Ragdoll;
			Rotation = key.Rotation;
			Time = key.Time;
			Translation = key.Translation;
		}

		#endregion //Methods

		#region File IO

#if !BRIDGE
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
						Translation = value.ToVector2() * Scale;
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

		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("key");

			if (!SkipTime)
			{
				xmlWriter.WriteAttributeString("time", Time.ToString());
			}

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

		[JsonIgnore]
		public bool SkipTime => 0 == Time;

		[JsonIgnore]
		public bool SkipRotation => 0f == Rotation;

		[JsonIgnore]
		public bool SkipLayer => 0 == Layer;

		[JsonIgnore]
		public bool SkipImage => string.IsNullOrEmpty(Image);

		[JsonIgnore]
		public bool SkipFlip => false == Flip;

		[JsonIgnore]
		public bool SkipTranslation => Vector2.Zero == Translation;

		[JsonIgnore]
		public bool SkipRagDoll => false == Ragdoll;

		#endregion //File IO
	}
}