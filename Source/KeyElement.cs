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
	class KeyXMLSort : IComparer<AnimationLib.KeyXML>
	{
		public int Compare(AnimationLib.KeyXML key1, AnimationLib.KeyXML key2)
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
		private Vector2 m_Translation;

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
			get { return m_Translation; }
			set { m_Translation = value; }
		}

		public float TranslationX
		{
			set { m_Translation.X = value; }
		}

		public float TranslationY
		{
			set { m_Translation.Y = value; }
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
			m_Translation = new Vector2(0.0f);
			Time = 0;
			Rotation = 0.0f;
			Layer = -1;
			ImageIndex = -1;
			Flip = false;
			KeyFrame = false;
			RagDoll = false;
		}

		public void Copy(KeyElement rInst)
		{
			m_Translation.X = rInst.m_Translation.X;
			m_Translation.Y = rInst.m_Translation.Y;
			Time = rInst.Time;
			Rotation = rInst.Rotation;
			Layer = rInst.Layer;
			ImageIndex = rInst.ImageIndex;
			Flip = rInst.Flip;
			KeyFrame = rInst.KeyFrame;
			RagDoll = rInst.RagDoll;
		}

		public bool Compare(KeyElement rInst)
		{
			if (Rotation != rInst.Rotation)
			{
				return false;
			}
			else if (Layer != rInst.Layer)
			{
				return false;
			}
			else if (ImageIndex != rInst.ImageIndex)
			{
				return false;
			}
			else if (Flip != rInst.Flip)
			{
				return false;
			}
			else if (Translation.X != rInst.Translation.X)
			{
				return false;
			}
			else if (Translation.Y != rInst.Translation.Y)
			{
				return false;
			}
			else if (RagDoll != rInst.RagDoll)
			{
				return false;
			}
			else if (JointName != rInst.JointName)
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
		/// <param name="strOldName">the name of the joint to be renamed</param>
		/// <param name="strNewName">the new name for that joint.</param>
		public void RenameJoint(string strOldName, string strNewName)
		{
			if (JointName == strOldName)
			{
				JointName = strNewName;
			}
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXMLFormat(XmlNode rXMLNode)
		{
			KeyFrame = true;

			if ("Item" != rXMLNode.Name)
			{
				Debug.Assert(false);
				return false;
			}

			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = rXMLNode.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				//will only have the name attribute
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;
				if ("Type" == strName)
				{
					if ("AnimationLib.KeyXML" != strValue)
					{
						Debug.Assert(false);
						return false;
					}
				}
			}

			//Read in child nodes
			if (rXMLNode.HasChildNodes)
			{
				for (XmlNode childNode = rXMLNode.FirstChild;
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
							m_Translation = strValue.ToVector2();
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

			return true;
		}

		/// <summary>
		/// write all this dude's stuff out to xml
		/// </summary>
		/// <param name="rXMLDude">the animtion object to add all the keyframes to</param>
		public void WriteXMLFormat(AnimationLib.AnimationXML rXMLDude, Bone rMyBone)
		{
			Debug.Assert(null != rMyBone);

			//find the image this key element uses
			string strImage = "";
			if (-1 != ImageIndex)
			{
				//find the bone that uses this dudes joint as a keyjoint
				Bone rChildBone = rMyBone.GetBone(JointName);
				if (null != rChildBone)
				{
					Debug.Assert(ImageIndex < rChildBone.Images.Count);
					strImage = rChildBone.Images[ImageIndex].ImageFile.GetFile();
				}
			}

			//create the xml object and add it to the animation
			AnimationLib.KeyXML myThing = new AnimationLib.KeyXML();
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
			myThing.translation = m_Translation;

			rXMLDude.keys.Add(myThing);
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			Layer *= iMultiply;
		}

		public void ReadSerializedFormat(AnimationLib.KeyXML rKeyElement)
		{
			KeyFrame = true;
			Flip = rKeyElement.flip;
			ImageName = rKeyElement.image;
			JointName = rKeyElement.joint;
			Layer = rKeyElement.layer;
			RagDoll = rKeyElement.ragdoll;
			Rotation = MathHelper.ToRadians(rKeyElement.rotation);
			Time = rKeyElement.time;
			m_Translation = rKeyElement.translation;
		}

		#endregion //File IO
	}
}