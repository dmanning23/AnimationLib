using System;
using System.Xml;
using Microsoft.Xna.Framework;

namespace AnimationLib
{
	public class JointData
	{
		#region Members

		/// <summary>
		/// the joint location
		/// </summary>
		private Vector2 m_Location;

		/// <summary>
		/// the first rotation limit for ragdoll
		/// </summary>
		private float m_FirstLimit;

		/// <summary>
		/// the second rotation limit for ragdoll
		/// </summary>
		private float m_SecondLimit;

		/// <summary>
		/// the first rotation limit for ragdoll when parent bone is flipped
		/// </summary>
		private float m_FirstLimitFlipped;

		/// <summary>
		/// the second rotation limit for ragdoll when parent bone is flipped
		/// </summary>
		private float m_SecondLimitFlipped;

		/// <summary>
		/// The radius of the circle that ragdoll is allowed to float around
		/// </summary>
		private float m_fFloatRadius;

		/// <summary>
		/// Whether this joint will use floating or rotating ragdoll
		/// Float uses the ragdollradius to float around the anchor joint
		/// chained uses the limits to rotate around the anchor joint
		/// </summary>
		private bool m_bFloatRagdoll;

		/// <summary>
		/// The distance from teh anchor position to this joint position
		/// </summary>
		private float m_fLength;

		/// <summary>
		/// This is the vector from this joint position to the anchor location
		/// </summary>
		private Vector2 m_AnchorVect;

		#endregion //Members

		#region Properties

		/// <summary>
		/// get the joint location
		/// </summary>
		public Vector2 Location
		{
			get { return m_Location; }
			set { m_Location = value; }
		}

		public float Length
		{
			get { return m_fLength; }
		}

		public Vector2 AnchorVect
		{
			get { return m_AnchorVect; }
		}

		public bool Floating
		{
			get { return m_bFloatRagdoll; }
			set { m_bFloatRagdoll = value; }
		}

		public float FloatRadius
		{
			get { return m_fFloatRadius; }
			set 
			{ 
				m_fFloatRadius = value; 
			}
		}

		public float FirstLimit
		{
			get { return m_FirstLimit; }
			set 
			{ 
				m_FirstLimit = value;
				UpdateFlippedLimits();
			}
		}

		public float SecondLimit
		{
			get { return m_SecondLimit; }
			set 
			{ 
				m_SecondLimit = value;
				UpdateFlippedLimits();
			}
		}

		public float FirstLimitFlipped
		{
			get { return m_FirstLimitFlipped; }
		}

		public float SecondLimitFlipped
		{
			get { return m_SecondLimitFlipped; }
		}

		#endregion

		#region Methods

		public JointData()
		{
			m_Location = new Vector2(0.0f);
			m_AnchorVect = new Vector2(0.0f);
			m_fLength = 0.0f;

			//user -180 and 180 as default limits
			FirstLimit = -MathHelper.Pi;
			SecondLimit = MathHelper.Pi;

			m_fFloatRadius = 0.0f;
			m_bFloatRagdoll = false;
		}

		public void Copy(JointData myInst)
		{
			m_Location = myInst.m_Location;
			FirstLimit = myInst.FirstLimit;
			SecondLimit = myInst.SecondLimit;
			m_fFloatRadius = myInst.FloatRadius;
			m_bFloatRagdoll = myInst.m_bFloatRagdoll;
			m_fLength = myInst.m_fLength;
			m_AnchorVect = myInst.m_AnchorVect;
		}

		/// <summary>
		/// Called when one of the limits is set to update the flipped limits
		/// </summary>
		private void UpdateFlippedLimits()
		{
			//Subtract the limits from 180 and switch them
			m_SecondLimitFlipped = -1.0f * m_FirstLimit;
			m_FirstLimitFlipped = -1.0f * m_SecondLimit;
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadSerializedFormat(XmlNode rXMLNode, Image myImage)
		{
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
					if ("AnimationLib.JointDataXML" != strValue)
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

					if (strName == "location")
					{
						Location = CStringUtils.ReadVectorFromString(strValue);
					}
					else if (strName == "limit1")
					{
						float fMyLimit = Convert.ToSingle(strValue);
						FirstLimit = MathHelper.ToRadians(fMyLimit);
					}
					else if (strName == "limit2")
					{
						float fMyLimit = Convert.ToSingle(strValue);
						m_SecondLimit = MathHelper.ToRadians(fMyLimit);
					}
					else if (strName == "FloatRadius")
					{
						m_fFloatRadius = Convert.ToSingle(strValue);
					}
					else if (strName == "FloatOrRotate")
					{
						m_bFloatRagdoll = Convert.ToBoolean(strValue);
					}
				}
			}

			//get the vector from the anchor position to this joint position
			m_AnchorVect = Location - myImage.AnchorCoord;
			m_fLength = m_AnchorVect.Length();

			return true;
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="rXMLFile">the xml file to add this dude as a child of</param>
		public void WriteXMLFormat(XmlTextWriter rXMLFile, float fEnbiggify)
		{
			//write out the item tag
			rXMLFile.WriteStartElement("Item");
			rXMLFile.WriteAttributeString("Type", "AnimationLib.JointDataXML");

			//write out joint offset
			rXMLFile.WriteStartElement("location");
			rXMLFile.WriteString((m_Location.X * fEnbiggify).ToString() + " " +
				(m_Location.Y * fEnbiggify).ToString());
			rXMLFile.WriteEndElement();

			//write first limit 
			rXMLFile.WriteStartElement("limit1");
			float fLimit1 = Helper.ClampAngle(m_FirstLimit);
			rXMLFile.WriteString(MathHelper.ToDegrees(fLimit1).ToString());
			rXMLFile.WriteEndElement();

			//write 2nd limit 
			rXMLFile.WriteStartElement("limit2");
			float fLimit2 = Helper.ClampAngle(m_SecondLimit);
			rXMLFile.WriteString(MathHelper.ToDegrees(fLimit2).ToString());
			rXMLFile.WriteEndElement();

			//write out float radius
			rXMLFile.WriteStartElement("FloatRadius");
			rXMLFile.WriteString((FloatRadius * fEnbiggify).ToString());
			rXMLFile.WriteEndElement();

			//write out whether it uses float or rotate ragdoll
			rXMLFile.WriteStartElement("FloatOrRotate");
			rXMLFile.WriteString(m_bFloatRagdoll ? "true" : "false");
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();
		}

		/// <summary>
		/// read this asshole from a serialized xml object
		/// </summary>
		/// <param name="rData">the thing to get data from</param>
		public bool ReadSerializedFormat(AnimationLib.JointDataXML rData, Image myImage)
		{
			Location = rData.location;
			FirstLimit = MathHelper.ToRadians(rData.limit1);
			SecondLimit = MathHelper.ToRadians(rData.limit2);
			m_fFloatRadius = rData.FloatRadius;
			m_bFloatRagdoll = rData.FloatOrRotate;

			//get the vector from the anchor position to this joint position
			m_AnchorVect = Location - myImage.AnchorCoord;
			m_fLength = m_AnchorVect.Length();
			return true;
		}

		#endregion
	}
}