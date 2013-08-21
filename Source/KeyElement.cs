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
		/// the time of this key element, stored in 1/60ths of a second
		/// </summary>
		private int m_iTime;

		/// <summary>
		/// the angle of rotation
		/// </summary>
		private float m_fRotation;

		/// <summary>
		/// the offset from the parents layer to draw at
		/// </summary>
		private int m_iLayer;

		/// <summary>
		/// teh name of the image to use, will be filename with no path info
		/// </summary>
		private string m_strImageName;

		/// <summary>
		/// the index of teh image to draw
		/// </summary>
		private int m_iImageIndex;

		/// <summary>
		/// whether it should be reversed on the x plane(drawn backwards)
		/// </summary>
		private bool m_bFlip;

		/// <summary>
		/// the bool flags the element as a keyframe
		/// this is needed for recalibrating the animation
		/// </summary>
		private bool m_bKeyFrame;

		/// <summary>
		/// the translation for this frame
		/// </summary>
		private Vector2 m_Translation;

		/// <summary>
		/// whether or not to turn on ragdoll physics
		/// </summary>
		private bool m_bRagDoll;

		/// <summary>
		/// The name of the joint that this keyframe describes
		/// </summary>
		private string m_strJointName;

		#endregion

		#region Properties

		/// <summary>
		/// Get or set the m_fTime member variable
		/// </summary>
		public int Time
		{
			get { return m_iTime; }
			set { m_iTime = value; }
		}

		/// <summary>
		/// Get or set the m_fRotation member variable
		/// </summary>
		public float Rotation
		{
			get { return m_fRotation; }
			set { m_fRotation = value; }
		}

		/// <summary>
		/// Get or set the m_iLayer member variable
		/// </summary>
		public int Layer
		{
			get { return m_iLayer; }
			set { m_iLayer = value; }
		}

		/// <summary>
		/// Get or set the m_iFrameIndex member variable
		/// </summary>
		public int ImageIndex
		{
			get { return m_iImageIndex; }
			set { m_iImageIndex = value; }
		}

		public string ImageName
		{
			get { return m_strImageName; }
		}

		/// <summary>
		/// Get or set the m_bFlip member variable
		/// </summary>
		public bool Flip
		{
			get { return m_bFlip; }
			set { m_bFlip = value; }
		}

		/// <summary>
		/// Get or set the m_bKeyFrame member variable
		/// </summary>
		public bool KeyFrame
		{
			get { return m_bKeyFrame; }
			set { m_bKeyFrame = value; }
		}

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
		/// Get or set the ragdoll member variable
		/// </summary>
		public bool RagDoll
		{
			get { return m_bRagDoll; }
			set { m_bRagDoll = value; }
		}

		public string JointName
		{
			get { return m_strJointName; }
			set { m_strJointName = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public KeyElement()
		{
			m_Translation = new Vector2(0.0f);
			m_iTime = 0;
			m_fRotation = 0.0f;
			m_iLayer = -1;
			m_iImageIndex = -1;
			m_bFlip = false;
			m_bKeyFrame = false;
			m_bRagDoll = false;
		}

		public void Copy(KeyElement rInst)
		{
			m_Translation.X = rInst.m_Translation.X;
			m_Translation.Y = rInst.m_Translation.Y;
			m_iTime = rInst.m_iTime;
			m_fRotation = rInst.m_fRotation;
			m_iLayer = rInst.m_iLayer;
			m_iImageIndex = rInst.m_iImageIndex;
			m_bFlip = rInst.m_bFlip;
			m_bKeyFrame = rInst.m_bKeyFrame;
			m_bRagDoll = rInst.m_bRagDoll;
		}

		public bool Compare(KeyElement rInst)
		{
			if (m_fRotation != rInst.m_fRotation)
			{
				return false;
			}
			else if (m_iLayer != rInst.m_iLayer)
			{
				return false;
			}
			else if (m_iImageIndex != rInst.m_iImageIndex)
			{
				return false;
			}
			else if (m_bFlip != rInst.m_bFlip)
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
			else if (m_bRagDoll != rInst.m_bRagDoll)
			{
				return false;
			}
			else if (m_strJointName != rInst.m_strJointName)
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
			if (m_strJointName == strOldName)
			{
				m_strJointName = strNewName;
			}
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadSerializedFormat(XmlNode rXMLNode)
		{
			m_bKeyFrame = true;

			if ("Item" != rXMLNode.Name)
			{
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

					if (strName == "time")
					{
						m_iTime = Convert.ToInt32(strValue);
					}
					else if (strName == "rotation")
					{
						m_fRotation = MathHelper.ToRadians(Convert.ToSingle(strValue));
					}
					else if (strName == "layer")
					{
						m_iLayer = Convert.ToInt32(strValue);
					}
					else if (strName == "image")
					{
						m_strImageName = strValue;
					}
					else if (strName == "flip")
					{
						m_bFlip = Convert.ToBoolean(strValue);
					}
					else if (strName == "translation")
					{
						m_Translation = strValue.ToVector2();
					}
					else if (strName == "ragdoll")
					{
						m_bRagDoll = Convert.ToBoolean(strValue);
					}
					else if (strName == "joint")
					{
						m_strJointName = strValue;
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
			if (-1 != m_iImageIndex)
			{
				//find the bone that uses this dudes joint as a keyjoint
				Bone rChildBone = rMyBone.GetBone(m_strJointName);
				if (null != rChildBone)
				{
					Debug.Assert(m_iImageIndex < rChildBone.Images.Count);
					strImage = rChildBone.Images[m_iImageIndex].Filename.GetFile();
				}
			}

			//create the xml object and add it to the animation
			AnimationLib.KeyXML myThing = new AnimationLib.KeyXML();
			myThing.flip = m_bFlip;
			myThing.image = strImage;
			myThing.joint = m_strJointName;
			myThing.layer = m_iLayer;
			myThing.ragdoll = m_bRagDoll;

			//Set the rotation to 0 if this dude is using ragdoll
			if (!m_bRagDoll)
			{
				myThing.rotation = MathHelper.ToDegrees(m_fRotation);
			}
			myThing.time = m_iTime;
			myThing.translation = m_Translation;

			rXMLDude.keys.Add(myThing);
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			m_iLayer *= iMultiply;
		}

		public void ReadSerializedFormat(AnimationLib.KeyXML rKeyElement)
		{
			m_bKeyFrame = true;
			m_bFlip = rKeyElement.flip;
			m_strImageName = rKeyElement.image;
			m_strJointName = rKeyElement.joint;
			m_iLayer = rKeyElement.layer;
			m_bRagDoll = rKeyElement.ragdoll;
			m_fRotation = MathHelper.ToRadians(rKeyElement.rotation);
			m_iTime = rKeyElement.time;
			m_Translation = rKeyElement.translation;
		}

		#endregion //File IO
	}
}