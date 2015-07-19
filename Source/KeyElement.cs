using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Microsoft.Xna.Framework;
using Vector2Extensions;

namespace AnimationLib
{
	/// <summary>
	/// class for sorting the keyframe XML by time
	/// </summary>
	class KeyXmlSort : IComparer<KeyXml>
	{
		public int Compare(KeyXml key1, KeyXml key2)
		{
			return key1.time.CompareTo(key2.time);
		}
	}

	/// <summary>
	/// class for sorting the keyframes by time
	/// </summary>
	class KeyElementSort : IComparer<KeyElement>
	{
		public int Compare(KeyElement key1, KeyElement key2)
		{
			return key1.Time.CompareTo(key2.Time);
		}
	}

	public class KeyElement
	{
		#region Member Variables

		/// <summary>
		/// the translation for this frame
		/// </summary>
		private Vector2 _translation;

		#endregion

		#region Properties

		/// <summary>
		/// the time of this key element, stored in 1/60ths of a second
		/// </summary>
		public int Time { get; set; }

		/// <summary>
		/// the angle of rotation, stored in radians
		/// </summary>
		public float Rotation { get; set; }

		/// <summary>
		/// the offset from the parents layer to draw at
		/// </summary>
		public int Layer { get; set; }

		/// <summary>
		/// the index of teh image to draw
		/// </summary>
		public int ImageIndex { get; set; }

		/// <summary>
		/// teh name of the image to use, will be filename with no path info
		/// </summary>
		public string ImageName { get; set; }

		/// <summary>
		/// whether it should be reversed on the x plane(drawn backwards)
		/// </summary>
		public bool Flip { get; set; }

		/// <summary>
		/// the bool flags the element as a keyframe
		/// this is needed for recalibrating the animation
		/// </summary>
		public bool KeyFrame { get; set; }

		/// <summary>
		/// Get or set the m_Translation member variable
		/// </summary>
		public Vector2 Translation
		{
			get { return _translation; }
			set { _translation = value; }
		}

		public float TranslationX
		{
			set { _translation.X = value; }
		}

		public float TranslationY
		{
			set { _translation.Y = value; }
		}

		/// <summary>
		/// whether or not to turn on ragdoll physics
		/// </summary>
		public bool RagDoll { get; set; }

		/// <summary>
		/// The name of the joint that this keyframe describes
		/// </summary>
		public string JointName { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public KeyElement()
		{
			_translation = Vector2.Zero;
			Time = 0;
			Rotation = 0.0f;
			Layer = -1;
			ImageIndex = -1;
			Flip = false;
			KeyFrame = false;
			RagDoll = false;
		}

		public void Copy(KeyElement inst)
		{
			_translation = inst._translation;
			Time = inst.Time;
			Rotation = inst.Rotation;
			Layer = inst.Layer;
			ImageIndex = inst.ImageIndex;
			Flip = inst.Flip;
			KeyFrame = inst.KeyFrame;
			RagDoll = inst.RagDoll;
		}

		public bool Compare(KeyElement inst)
		{
			if (Rotation != inst.Rotation)
			{
				return false;
			}
			else if (Layer != inst.Layer)
			{
				return false;
			}
			else if (ImageIndex != inst.ImageIndex)
			{
				return false;
			}
			else if (Flip != inst.Flip)
			{
				return false;
			}
			else if (Translation.X != inst.Translation.X)
			{
				return false;
			}
			else if (Translation.Y != inst.Translation.Y)
			{
				return false;
			}
			else if (RagDoll != inst.RagDoll)
			{
				return false;
			}
			else if (JointName != inst.JointName)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="oldName">the name of the joint to be renamed</param>
		/// <param name="newName">the new name for that joint.</param>
		public void RenameJoint(string oldName, string newName)
		{
			if (JointName == oldName)
			{
				JointName = newName;
			}
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="node">The xml node to read from</param>
		public void ReadXmlFormat(XmlNode node)
		{
			KeyFrame = true;

#if DEBUG
			if ("Item" != node.Name)
			{
				throw new Exception("keyelement nodes need to be Item not " + node.Name);
			}

			if (!node.HasChildNodes)
			{
				throw new Exception("keyelement with no child nodes");
			}

			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = node.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				//will only have the name attribute
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;

				if ("Type" == strName)
				{
					if ("AnimationLib.KeyXML" != strValue)
					{
						throw new Exception("keyelement needs to be AnimationLib.KeyXML not " + strValue);
					}
				}
			}
#endif

			//Read in child nodes
			for (XmlNode childNode = node.FirstChild;
				null != childNode;
				childNode = childNode.NextSibling)
			{
				//what is in this node?
				string strName = childNode.Name;
				string strValue = childNode.InnerText;

				switch (strName)
				{
					case "time":
					{
						Time = Convert.ToInt32(strValue);
					}
					break;
					case "rotation":
					{
						Rotation = MathHelper.ToRadians(Convert.ToSingle(strValue));
					}
					break;
					case "layer":
					{
						Layer = Convert.ToInt32(strValue);
					}
					break;
					case "image":
					{
						ImageName = strValue;
					}
					break;
					case "flip":
					{
						Flip = Convert.ToBoolean(strValue);
					}
					break;
					case "translation":
					{
						Translation = strValue.ToVector2();
					}
					break;
					case "ragdoll":
					{
						RagDoll = Convert.ToBoolean(strValue);
					}
					break;
					case "joint":
					{
						JointName = strValue;
					}
					break;
				}
			}
		}

		/// <summary>
		/// write all this dude's stuff out to xml
		/// </summary>
		/// <param name="animationXml">the animtion object to add all the keyframes to</param>
		/// <param name="myBone">the bone this dude references</param>
		public void WriteXmlFormat(AnimationXml animationXml, Bone myBone)
		{
			Debug.Assert(null != myBone);

			//find the image this key element uses
			string strImage = "";
			if (-1 != ImageIndex)
			{
				//find the bone that uses this dudes joint as a keyjoint
				Bone rChildBone = myBone.GetBone(JointName);
				if (null != rChildBone)
				{
					Debug.Assert(ImageIndex < rChildBone.Images.Count);
					strImage = rChildBone.Images[ImageIndex].ImageFile.GetFile();
				}
			}

			//create the xml object and add it to the animation
			var myThing = new KeyXml();
			myThing.flip = Flip;
			myThing.image = strImage;
			myThing.joint = JointName;
			myThing.layer = Layer;
			myThing.ragdoll = RagDoll;

			//Set the rotation to 0 if this dude is using ragdoll
			if (!RagDoll)
			{
				myThing.rotation = MathHelper.ToDegrees(Rotation);
			}
			myThing.time = Time;
			myThing.translation = Translation;

			animationXml.keys.Add(myThing);
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			Layer *= multiply;
		}

		#endregion //File IO
	}
}