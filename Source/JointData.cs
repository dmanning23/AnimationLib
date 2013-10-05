using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Vector2Extensions;

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
		/// user -180 and 180 as default limits
		/// </summary>
		private float m_FirstLimit = -MathHelper.Pi;

		/// <summary>
		/// the second rotation limit for ragdoll
		/// user -180 and 180 as default limits
		/// </summary>
		private float m_SecondLimit = MathHelper.Pi;

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

		/// <summary>
		/// The distance from teh anchor position to this joint position
		/// </summary>
		public float Length { get; private set; }

		public Vector2 AnchorVect
		{
			get { return m_AnchorVect; }
		}

		/// <summary>
		/// Whether this joint will use floating or rotating ragdoll
		/// Float uses the ragdollradius to float around the anchor joint
		/// chained uses the limits to rotate around the anchor joint
		/// </summary>
		public bool Floating { get; set; }

		/// <summary>
		/// The radius of the circle that ragdoll is allowed to float around
		/// </summary>
		public float FloatRadius { get; set; }

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

		/// <summary>
		/// the first rotation limit for ragdoll when parent bone is flipped
		/// </summary>
		public float FirstLimitFlipped { get; private set; }

		/// <summary>
		/// the second rotation limit for ragdoll when parent bone is flipped
		/// </summary>
		public float SecondLimitFlipped { get; private set; }

		#endregion

		#region Methods

		public JointData()
		{
			m_Location = new Vector2(0.0f);
			m_AnchorVect = new Vector2(0.0f);
			Length = 0.0f;
			FloatRadius = 0.0f;
			Floating = false;
		}

		public void Copy(JointData myInst)
		{
			m_Location = myInst.m_Location;
			FirstLimit = myInst.FirstLimit;
			SecondLimit = myInst.SecondLimit;
			FloatRadius = myInst.FloatRadius;
			Floating = myInst.Floating;
			Length = myInst.Length;
			m_AnchorVect = myInst.m_AnchorVect;
		}

		/// <summary>
		/// Called when one of the limits is set to update the flipped limits
		/// </summary>
		private void UpdateFlippedLimits()
		{
			//Subtract the limits from 180 and switch them
			SecondLimitFlipped = -1.0f * FirstLimit;
			FirstLimitFlipped = -1.0f * SecondLimit;
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXMLFormat(XmlNode rXMLNode, Image myImage)
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
						Location = strValue.ToVector2();
					}
					else if (strName == "limit1")
					{
						float fMyLimit = Convert.ToSingle(strValue);
						FirstLimit = MathHelper.ToRadians(fMyLimit);
					}
					else if (strName == "limit2")
					{
						float fMyLimit = Convert.ToSingle(strValue);
						SecondLimit = MathHelper.ToRadians(fMyLimit);
					}
					else if (strName == "FloatRadius")
					{
						FloatRadius = Convert.ToSingle(strValue);
					}
					else if (strName == "FloatOrRotate")
					{
						Floating = Convert.ToBoolean(strValue);
					}
				}
			}

			//get the vector from the anchor position to this joint position
			m_AnchorVect = Location - myImage.AnchorCoord;
			Length = m_AnchorVect.Length();

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
			float fLimit1 = Helper.ClampAngle(FirstLimit);
			rXMLFile.WriteString(MathHelper.ToDegrees(fLimit1).ToString());
			rXMLFile.WriteEndElement();

			//write 2nd limit 
			rXMLFile.WriteStartElement("limit2");
			float fLimit2 = Helper.ClampAngle(SecondLimit);
			rXMLFile.WriteString(MathHelper.ToDegrees(fLimit2).ToString());
			rXMLFile.WriteEndElement();

			//write out float radius
			rXMLFile.WriteStartElement("FloatRadius");
			rXMLFile.WriteString((FloatRadius * fEnbiggify).ToString());
			rXMLFile.WriteEndElement();

			//write out whether it uses float or rotate ragdoll
			rXMLFile.WriteStartElement("FloatOrRotate");
			rXMLFile.WriteString(Floating ? "true" : "false");
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
			FloatRadius = rData.FloatRadius;
			Floating = rData.FloatOrRotate;

			//get the vector from the anchor position to this joint position
			m_AnchorVect = Location - myImage.AnchorCoord;
			Length = m_AnchorVect.Length();
			return true;
		}

		#endregion
	}
}